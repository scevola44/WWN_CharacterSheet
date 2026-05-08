# Plan: Art.Source → ArtSource Lookup Table

## Design decisions confirmed

| Question | Answer |
|---|---|
| Is `Art.Source` engine-relevant (rules branch on it)? | **No** — purely display metadata; no C# code branches on its value |
| Modelling approach | **DB-backed lookup table** (`ArtSource` entity, int PK) |
| Runtime extensibility | **Admin-extensible** — full CRUD endpoints + admin UI |
| Seeded rows | `{1, "Mage", "Mage"}` and `{2, "PartialMage", "Partial Mage"}` |

---

## Phase 1 — Domain: `ArtSource` entity + `Art` model change

### New file: `src/WWN.Domain/Entities/ArtSource.cs`
- `int Id` (PK, stable/manual — not DB-generated)
- `string Code` (max 50, unique — machine key used in API/migrations)
- `string DisplayName` (max 100 — shown in UI)
- `string? Description`
- `int SortOrder`
- Private setters; public `Update(displayName, description, sortOrder)` mutator following domain conventions.

### Modify: `src/WWN.Domain/Entities/Art.cs`
- Remove `string Source` property, constructor param, and `Update(...)` param.
- Add `int SourceId` property, constructor param, and `Update(...)` param.
- Add optional navigation `ArtSource? SourceNavigation` (EF use only — no business logic reads it directly).

---

## Phase 2 — Infrastructure: EF config + migration

### New file: `src/WWN.Infrastructure/Persistence/Configurations/ArtSourceConfiguration.cs`
- `ToTable("ArtSources")`
- `HasKey(x => x.Id)` with `ValueGeneratedNever()` (stable manual IDs)
- Unique index on `Code`
- `HasData` seed: `{Id=1, Code="Mage", DisplayName="Mage", SortOrder=1}`, `{Id=2, Code="PartialMage", DisplayName="Partial Mage", SortOrder=2}`

### Modify: `src/WWN.Infrastructure/Persistence/Configurations/ArtConfiguration.cs`
- Replace `.Property(x => x.Source)` with `.Property(x => x.SourceId).IsRequired()`
- Add `HasOne(x => x.SourceNavigation).WithMany().HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.Restrict)`

### Add migration: `AddArtSourceLookupTable`
SQLite requires a full table rebuild to add a NOT NULL FK column. The migration will:
1. Create `ArtSources` table and insert seed rows.
2. `PRAGMA foreign_keys=OFF`
3. Create `Arts_new` with `SourceId INTEGER NOT NULL` (FK to ArtSources).
4. Backfill:
   ```sql
   INSERT INTO Arts_new (Id, Name, Description, Summary, MinLevel, EffortCost, SourceId)
   SELECT Id, Name, Description, Summary, MinLevel, EffortCost,
     CASE WHEN Source = 'PartialMage' THEN 2 ELSE 1 END
   FROM Arts;
   ```
   (Any unrecognised string defaults to `Mage`/1 — acceptable; the existing DB only has "Mage"/"PartialMage" strings.)
5. `DROP TABLE Arts; ALTER TABLE Arts_new RENAME TO Arts`
6. `PRAGMA foreign_keys=ON`
7. Down migration reverses: rebuild Arts with `Source TEXT` backfilled from `ArtSources.Code`.

---

## Phase 3 — Application layer: DTOs + services

### Modify: `src/WWN.Application/DTOs/CharacterDtos.cs`
- `ArtDto`: `string Source` → `int SourceId`

### Modify: `src/WWN.Application/DTOs/RequestDtos.cs`
- `CreateArtRequest` / `UpdateArtRequest`: `string Source` → `int SourceId`

### Modify: `src/WWN.Application/Services/ArtService.cs`
- Update `MapToDto` (use `SourceId`), and `CreateArtAsync` / `UpdateArtAsync` (pass `SourceId`).

### Modify: `src/WWN.Application/DTOs/LookupsDto.cs` (or wherever it lives)
- Add `LookupValueDto[] ArtSources { get; init; }` to `LookupsDto`.

### Modify: `src/WWN.Application/Services/LookupsService.cs`
- Remove process-static cache for the whole `LookupsDto` (it's now partly DB-backed).
- Inject `WwnDbContext` (or a repo interface).
- Fetch `ArtSource` rows from DB on each call (SELECT on a tiny table — negligible cost).
- Compose `LookupsDto` from static `EffortCommitmentCatalog` + DB-fetched `ArtSources`.
- Recompute ETag from the combined payload so HTTP 304 still works.
- Service lifetime changes from singleton-static to **scoped**.

### New file: `src/WWN.Application/Services/ArtSourceService.cs`
- `ListAsync()` → all rows ordered by SortOrder
- `CreateAsync(code, displayName, description?, sortOrder)` → validates Code uniqueness, returns new `ArtSourceDto`
- `UpdateAsync(id, displayName, description?, sortOrder)` → 404 if not found
- `DeleteAsync(id)` → 409 Conflict if any `Arts.SourceId` references this row; otherwise delete

---

## Phase 4 — Web API: endpoint updates + new ArtSource CRUD

### Modify: `src/WWN.Web/Endpoints/LookupsEndpoints.cs`
- Handler becomes `async` (DB call).
- Drop `Cache-Control: public, max-age=300`; use `Cache-Control: private, max-age=30` (data is mutable).
- Keep ETag / 304 logic (ETag now covers both static + DB data).

### New file: `src/WWN.Web/Endpoints/ArtSourceEndpoints.cs`
```
GET    /api/art-sources          → list all (public)
POST   /api/art-sources          → create (requires auth)
PUT    /api/art-sources/{id}     → update (requires auth)
DELETE /api/art-sources/{id}     → delete (requires auth); 409 if in use
```

### Modify: `src/WWN.Web/Program.cs`
- Register `ArtSourceService` (scoped).
- Map `ArtSourceEndpoints`.

---

## Phase 5 — Frontend: types, context, components, admin UI

### Modify: `src/WWN.Web/client-app/src/types/lookups.ts`
- Add `artSources: LookupValue[]` to `Lookups` interface.

### Modify: `src/WWN.Web/client-app/src/contexts/LookupsContext.tsx`
- Parse `artSources` array from API response.
- Build `artSourceById: Map<number, LookupValue>` and `artSourceByCode: Map<string, LookupValue>`.
- Export `useArtSources()` → `LookupValue[]` and `useArtSource(id: number)` → `LookupValue | undefined`.

### Modify: `src/WWN.Web/client-app/src/types/art.ts`
- `Art.source: string` → `Art.sourceId: number`
- Same change in `CreateArtRequest` and `UpdateArtRequest`.

### Modify: `src/WWN.Web/client-app/src/components/arts/ArtForm.tsx`
- Replace free-text `<input placeholder="Mage / PartialMage">` with `<select>` populated via `useArtSources()`.
- Default selection: first item in sorted list (or current `sourceId` when editing).

### Modify: `src/WWN.Web/client-app/src/pages/ArtDatabasePage.tsx`
- Source filter: text input → `<select>` with an "All sources" option plus one `<option>` per lookup row.
- Summary display line: `art.source` → `artSourceById.get(art.sourceId)?.displayName`.

### Modify: `src/WWN.Web/client-app/src/components/arts/ArtDetailModal.tsx`
- Display source name: `artSourceById.get(art.sourceId)?.displayName ?? '—'`.

### New file: `src/WWN.Web/client-app/src/pages/ArtSourceAdminPage.tsx`
- Table listing all source rows (Code, Display Name, Sort Order).
- Add / edit via inline form or small modal.
- Delete with confirmation; show error toast if 409 (source in use).
- Calls `/api/art-sources` CRUD endpoints.
- Refreshes LookupsContext after mutations (re-fetch `/api/lookups`).

### Modify: router / `App.tsx`
- Add route `/admin/art-sources` → `ArtSourceAdminPage`.
- Add a navigation link (e.g. alongside the existing Art Database link, visible only when authenticated).

---

## Phase 6 — Build & validate

```
dotnet build                                     # all C# projects
dotnet test                                      # domain + application + integration
cd src/WWN.Web/client-app && npm run build       # tsc -b + Vite bundle
cd src/WWN.Web/client-app && npm run lint        # ESLint
```

---

## Risks & open questions

| # | Risk | Mitigation |
|---|---|---|
| 1 | **Unrecognised Source strings in DB** | Migration CASE defaults to `Mage`. Verify no exotic strings exist before running. |
| 2 | **LookupsService lifetime change** | Moving from static-cached to scoped adds one DB query per `/api/lookups` call. Acceptable at this scale; `IMemoryCache` with TTL can be added later. |
| 3 | **Auth on ArtSource CRUD** | Existing Art write endpoints use `RequireAuthorization()`. ArtSource write endpoints will use the same pattern. Confirm no admin-role distinction is needed. |
| 4 | **Delete safety** | `ArtSourceService.DeleteAsync` returns 409 if referenced. No cascade. |
| 5 | **Admin UI discoverability** | A nav link on `ArtDatabasePage` or the main nav bar (guarded by `useAuth`) keeps the admin surface small and predictable. |
