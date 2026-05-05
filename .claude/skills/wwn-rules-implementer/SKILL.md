---
name: wwn-rules-implementer
description: Use when implementing or modifying any feature whose behavior depends on Worlds Without Number game rules (combat, magic, leveling, skills, encumbrance, strain, character creation, rest cycles). Forces a plan-first, retrieval-disciplined workflow before code is written.
---

# WWN Rules Implementer

You are about to write or change code that depends on WWN game rules in this repository. Do **not** start coding yet. Run the steps below in order.

## 1. Identify relevant rule files

- Open `.claude/rules/index.md` and use the "Minimum files to read for task X" table.
- List the files you intend to read **out loud** to the user (one bullet per file). If the task is not in the table, justify your selection in one sentence.
- Always read `app-decisions.md`. Project-specific rulings override the generic rules.

## 2. Extract the minimum needed rules

For each rule file you opened, copy/quote into the plan only:

- the formulas you will actually compute
- the enums/states you will branch on
- any cap, floor, or validation constraint
- relevant cross-references to existing C# in `src/WWN.Domain/Rules/` or `src/WWN.Domain/Aggregates/Character.cs`

Do **not** dump the whole rule file. If you find yourself summarizing a section that is not directly used, drop it.

## 3. List assumptions explicitly

Before designing anything, write a numbered list of assumptions, e.g.:

1. "I assume HP recovery on rest is `full heal on RestForDay`" (cite `app-decisions.md` Open item).
2. "I assume the user wants Initiative as `1d8 + DEX`."

Each assumption must reference either a confirmed entry in `app-decisions.md`, an Open TODO that you are resolving, or a question to ask the user. If anything is genuinely unclear, **ask the user** before proceeding.

## 4. Propose a plan

Produce a short plan with these sections:

- **Data model** — new fields, value objects, or migrations. Note EF Core implications (e.g. arrays must be reassigned, not mutated).
- **Domain logic** — which calculator/aggregate methods change, and what the new formula is. Prefer adding to `WWN.Domain/Rules/` for pure functions.
- **Application / API** — new endpoints, DTO changes, validation.
- **Frontend** — affected components in `src/WWN.Web/client-app/src/`, type changes in `types/`.
- **Tests** — which xUnit project gets new tests; list at least 2 edge cases.
- **Open questions** — anything still ambiguous after step 3. If non-empty, **stop and ask** before implementing.

Keep the plan tight — bullets, not prose.

## 5. Confirm, then implement

- Wait for the user's go-ahead on the plan.
- Implement in the order: domain → application → infrastructure (migration) → web/API → frontend → tests.
- After implementing, update `.claude/rules/app-decisions.md` if the work resolved an Open TODO or introduced a new project-specific decision.

## 6. Style guardrails

- Don't duplicate logic already in `WWN.Domain/Rules/` — extend it.
- Recompute derived values (BAB, AC, slots, Effort, save targets) on read; never persist them.
- Keep the Domain layer free of EF Core / ASP.NET dependencies.
- Frontend types in `client-app/src/types/` must mirror DTOs after any API change.
