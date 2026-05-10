# Issue 11: No Optimistic Concurrency Control

## Problem

`CharacterRepository.UpdateAsync` saves whatever is in the EF Core change tracker with no version check. Two browser tabs — or two parallel API requests — editing the same character will silently overwrite each other. The last write wins with no warning to either caller.

**Current code** (`src/WWN.Infrastructure/Repositories/CharacterRepository.cs`):
```csharp
public async Task UpdateAsync(Character character, CancellationToken cancellationToken = default)
{
    var entry = dbContext.Entry(character);
    if (entry.State == EntityState.Detached)
        dbContext.Characters.Update(character);
    await dbContext.SaveChangesAsync(cancellationToken);  // no version check
}
```

There is no `RowVersion`, `Timestamp`, or `IsConcurrencyToken` mapping anywhere in `CharacterConfiguration.cs`.

---

## Why it is non-trivial

EF Core's optimistic concurrency requires the *original* token value (the one the client held when it first loaded the character) to be sent back with every mutation request. Merely adding a column and loading it fresh in `GetByIdAsync` is not enough — the fresh load always has the latest version, so the WHERE clause would always match.

Full enforcement therefore requires:
1. **Backend**: add a Version column, expose it in `CharacterDetailDto`.
2. **Frontend**: track `version` in the React state and include it in every mutation request body.
3. **Backend**: accept `version` in request DTOs, set it on the entity before `SaveChangesAsync`, handle the resulting `DbUpdateConcurrencyException` → HTTP 409.

---

## Implementation steps

### Step 1 — Domain (no migration yet, just the property)

`src/WWN.Domain/Aggregates/Character.cs`:
```csharp
public uint Version { get; private set; }
public void IncrementVersion() => Version++;
```

### Step 2 — EF Core configuration

`src/WWN.Infrastructure/Persistence/Configurations/CharacterConfiguration.cs`:
```csharp
entity.Property(c => c.Version).IsConcurrencyToken();
```

SQLite does not support native `rowversion`/`timestamp`. Using a manually incremented `uint` with `IsConcurrencyToken()` is the correct SQLite approach.

Call `character.IncrementVersion()` inside `CharacterRepository.UpdateAsync` before `SaveChangesAsync`.

### Step 3 — EF Core migration

```
dotnet ef migrations add AddCharacterVersion \
  --project src/WWN.Infrastructure \
  --startup-project src/WWN.Web
```

### Step 4 — Expose in response DTO

`src/WWN.Application/DTOs/ResponseDtos.cs` — add to `CharacterDetailDto`:
```csharp
public uint Version { get; init; }
```

Map it in `CharacterService.MapToDetailDtoAsync`.

### Step 5 — Handle the exception in middleware

`src/WWN.Web/Middleware/ExceptionHandlingMiddleware.cs`:
```csharp
Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException
    => (StatusCodes.Status409Conflict, "Character was modified by another session — reload and retry."),
```

### Step 6 — Accept Version in mutation requests (frontend + backend)

This is the load-bearing step. Every mutation DTO that touches a character needs a `Version` field:
```csharp
public record UpdateAttributeRequest
{
    public int Score { get; init; }
    public uint Version { get; init; }   // added
}
// ... same for all other mutation DTOs ...
```

In `CharacterService`, after loading the character, set:
```csharp
// Force EF Core to treat the client-supplied version as the "original"
dbContext.Entry(character).Property(c => c.Version).OriginalValue = request.Version;
```

Now `SaveChangesAsync` issues `WHERE Id = ? AND Version = ?`. If another request already incremented `Version`, the update affects 0 rows → `DbUpdateConcurrencyException` → 409.

### Step 7 — Frontend

`src/WWN.Web/client-app/src/types/character.ts`:
- Add `version: number` to `CharacterDetail`.
- Include `version` in every mutation request payload.
- On 409 response, prompt the user to reload the sheet.

---

## Files touched

| File | Change |
|------|--------|
| `src/WWN.Domain/Aggregates/Character.cs` | Add `Version` + `IncrementVersion()` |
| `src/WWN.Infrastructure/Persistence/Configurations/CharacterConfiguration.cs` | `IsConcurrencyToken()` mapping |
| `src/WWN.Infrastructure/Repositories/CharacterRepository.cs` | Call `IncrementVersion()` before save; set `OriginalValue` from request |
| EF Core migration (new) | `AddCharacterVersion` |
| `src/WWN.Application/DTOs/RequestDtos.cs` | Add `Version` to every mutation DTO |
| `src/WWN.Application/DTOs/ResponseDtos.cs` | Add `Version` to `CharacterDetailDto` |
| `src/WWN.Application/Services/CharacterService.cs` | Pass `Version` from request to EF Core original-value override |
| `src/WWN.Web/Middleware/ExceptionHandlingMiddleware.cs` | Map `DbUpdateConcurrencyException` → 409 |
| `src/WWN.Web/client-app/src/types/character.ts` | Add `version` field |
| All frontend API call sites | Include `version` in request body |
