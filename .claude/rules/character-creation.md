# Character Creation

Source: free WWN rules, paraphrased.

## Raw rule summary

A character at creation has:

1. **Six attributes** (STR, DEX, CON, INT, WIS, CHA), each 3–18. WWN supports several generation methods (3d6 down the line, array, or "roll 3d6 six times and place"). One score may be raised to 14 if none of the rolled scores reaches 14.
2. **Background** — narrative origin granting one **free starting skill** plus additional learning rolls (skill table or growth/learning options). Mostly fluff for the app + mechanical skill-gain hooks.
3. **Class**: one of **Warrior**, **Expert**, **Mage**, or **Adventurer** (see `classes-and-leveling.md`).
   - Adventurer must pick **two distinct partial classes** (Partial Warrior / Partial Expert / Partial Mage).
4. **Foci**: every character starts with a free **Any focus** (or class-specific equivalent at GM option). Warriors/Experts/Mages and partial classes get extra foci according to class. Foci have levels 1 or 2.
5. **Skills**: starting skills come from background + class. Trained skills start at rank **0**; untrained sit at **−1** in this codebase's convention.
6. **HP**: at level 1, roll the class hit die (`1d6` baseline) + attribute (CON mod) + class hit-die modifier (Warrior +2, Mage −1, Expert/Adventurer 0; Adventurer follows partial-Warrior / partial-Mage). Minimum 1 HP per level.
7. **Equipment**: starting gear is class-flavored; not mechanically required by the codebase.

## App implications

- The factory `Character.Create()` (see `src/WWN.Domain/Aggregates/Character.cs:81`) already enforces:
  - Adventurer ⇒ both partial slots set; non-Adventurer ⇒ neither set.
  - Adventurer's two partial classes must be **distinct** (duplicate pair rejected).
  - All six attributes provided.
  - 21 default skills initialized at rank −1 (Custom is excluded).
  - Default `MaxHitPoints = 1` if not given.
- The current chargen page is `client-app/src/pages/CharacterCreatePage.tsx`; level is fixed at 1 by the factory.
- We do **not** currently support automated background skill rolls or starter foci selection — these are entered manually.

## Data model implications

- `Character` aggregate is the source of truth. Attribute scores, skill ranks, foci, partial classes are all stored on the aggregate.
- No "background" entity yet — only a free-text `Background` string. If we add structured backgrounds, they'd live as a definition table similar to `FocusDefinition` / `ClassAbilityDefinition`.

## UI implications

- A guided-chargen flow needs steps: attributes → class & partials → background → starting foci → starting skills → HP roll → review. Each step should validate before advancing.
- Adventurer requires two **different** partial classes; the domain enforces both the presence and distinctness of the pair.
- "Custom skill" must be available for backgrounds that grant niche skills.

## Open questions / ambiguities

- No starting-skill/free-focus wizard yet; likely belongs in Application layer as a `CharacterCreationService`.
- Attribute generation method is a UI concern — choose: manual entry only, optional in-app roller, or both.
- HP at level 1 is currently passed in by the caller; there is no helper to compute the level-1 max from die + CON. Worth adding if we build a chargen wizard.
