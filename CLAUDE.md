# CLAUDE.md

Webapp for managing characters in the **Worlds Without Number** TTRPG.
.NET backend (clean architecture) + React/TypeScript SPA, SQLite via EF Core.

## Working preferences

- Be very honest. Tell me something I need to know even if I don't want to hear it.
- Push back when something seems wrong — don't just agree with mistakes.
- Ask questions if something is not clear and you need to make a choice. Don't choose randomly — it's something important to what we're doing.
- When you show me a potential error or miss, start your response with the ❗️ emoji.
- **ALWAYS** start replies with a STARTER_CHARACTER + space (default: 🔮). Stack emojis when necessary, don't replace them.

## Coding standards

- **SOLID where it genuinely reduces coupling**, not as a goal in itself. Don't introduce interfaces, dependency injection, or inheritance hierarchies just to tick a box — only when they remove real pain.
- **Self-documenting names.** Variables, functions, types, and modules should read as prose. If you're about to write a comment that explains *what* a line does, rename the thing instead.
- **Comments explain *why*, not *what*.** Reserve them for non-obvious decisions, workarounds, edge cases, references to specs/issues, and business rules that the code alone can't express. Never paraphrase the next line.
- **YAGNI.** No speculative abstractions, config knobs, or "just-in-case" validation for requirements that don't exist yet. Build for what's asked; refactor when the second or third real use case arrives.
- **DRY, but wait for the rule of three.** Two similar blocks are duplication; three are a pattern. Premature deduplication couples things that only look alike.
- **Small, single-purpose functions.** If a function's name needs an "and", split it. If it doesn't fit on a screen, consider splitting it.
- **Prefer composition over inheritance.** Reach for inheritance only for true is-a relationships.
- **Validate at the system boundary** (user input, HTTP, filesystem, third-party APIs). Trust internal callers — don't defensively re-check types the type system already guarantees.
- **No magic numbers or strings.** Name them as constants with the meaning embedded in the name.
- **Match the conventions of the file you're editing** — formatting, naming, imports, patterns. Consistency beats personal preference.
- **Tests describe behavior, not implementation.** Assert on observable outcomes, not on internals the caller doesn't care about.
- **Type everything the language lets you type.** Full type hints in C#, no implicit `any` in TypeScript.
- **Use meaningful, descriptive variable names.** Variable names should clearly convey their purpose, such as `availableLeaveBalance` instead of `d`. This practice enhances readability and maintainability, reducing the need for comments and making the code more understandable for other developers.

## Stack

- **Backend:** .NET 10, C#, EF Core, SQLite. Projects: `WWN.Domain`, `WWN.Application`, `WWN.Infrastructure`, `WWN.Web` (Minimal API + endpoints).
- **Frontend:** React 19 + TypeScript + Vite, located at `src/WWN.Web/client-app/`. Axios for API calls, react-router-dom v7.
- **Tests:** xUnit projects under `tests/` (Domain, Application, Integration).

## Key commands

Run from repo root unless noted.

| Task | Command |
|------|---------|
| Build solution | `dotnet build` |
| Run all tests | `dotnet test` |
| Run backend (port 5000) | `dotnet run --project src/WWN.Web` |
| Frontend dev server | `cd src/WWN.Web/client-app && npm run dev` |
| Frontend build (typecheck + bundle) | `cd src/WWN.Web/client-app && npm run build` |
| Frontend lint | `cd src/WWN.Web/client-app && npm run lint` |
| EF Core migrations | `dotnet ef --project src/WWN.Infrastructure --startup-project src/WWN.Web migrations add <Name>` |

There is no separate frontend "test" or "typecheck" script — `npm run build` runs `tsc -b` first.

## Important directories

- `src/WWN.Domain/` — aggregates, entities, value objects, **rules calculators** (`Rules/*.cs`), enums. Pure C#, no infra deps.
- `src/WWN.Application/` — services, DTOs, factories, helpers. Orchestrates domain.
- `src/WWN.Infrastructure/` — EF Core persistence, repositories, Identity.
- `src/WWN.Web/Endpoints/` — API endpoints. `src/WWN.Web/client-app/src/` — React app (`pages/`, `components/`, `hooks/`, `api/`, `types/`).
- `tests/` — xUnit tests mirroring the project layout.
- `.claude/rules/` — **WWN game-rules knowledge** (modular). See `.claude/rules/index.md`.
- `.claude/skills/` — reusable workflows (e.g. `wwn-rules-implementer`).

## Working with WWN game rules

Game rules **live in `.claude/rules/`**, not in this file. Do not try to hold all of WWN in your head.

When a task touches game mechanics:

1. Open `.claude/rules/index.md` and read **only** the rules files relevant to the task.
2. Cross-check existing implementation in `src/WWN.Domain/Rules/` and `src/WWN.Domain/Aggregates/Character.cs` before writing new code — the codebase has already encoded a lot of mechanics and project-specific decisions are tracked in `.claude/rules/app-decisions.md`.
3. For non-trivial rule-heavy work (level-up flow, new spellcasting feature, attack pipeline changes, encumbrance), **propose a plan first** (data model + UI + validation + edge cases) and confirm before implementing. The `wwn-rules-implementer` skill formalizes this.
4. If a rule is ambiguous in the source material, flag it; do not invent specifics. Add to `app-decisions.md` once a ruling is chosen.

## Conventions

- Domain types are immutable from the outside (private setters, mutator methods on aggregates). Preserve this pattern.
- New mechanics belong in `WWN.Domain/Rules/` as static calculators when they are pure functions of character state.
- Frontend types in `client-app/src/types/` mirror backend DTOs — keep them in sync when changing endpoints.
- Skill ranks are stored in the range **-1..4** (untrained = -1). Levels are **1-10**. Attribute scores **3-18**.
