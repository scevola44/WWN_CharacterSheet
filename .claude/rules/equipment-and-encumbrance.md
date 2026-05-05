# Equipment and Encumbrance

Source: free WWN rules, paraphrased.

## Item slots

WWN uses a slot-based encumbrance system. Each item occupies one or more slots based on size/weight. The codebase exposes three slot types via `ItemSlotType`:

| Slot | Meaning |
|------|---------|
| `Stowed` | In a pack/bag; not immediately usable. Counts toward stowed capacity. |
| `Readied` | At hand (belt, holster). Quick to use. Counts toward readied capacity. |
| `Equipped` | Worn or wielded — armor on the body, weapon in hand, shield strapped. Most permissive but exclusive (one weapon, one armor, one shield at a time). |

## Carrying capacity (raw rule summary)

- **Readied capacity ≈ ½ STR score** items.
- **Stowed capacity ≈ STR score** items (additional, beyond readied).
- Exceeding capacity imposes encumbrance penalties (reduced movement, eventually unable to move). Exact penalty bands per rulebook.
- Heavy items take multiple slots; small/light items can be bundled.

> **Not modeled in code today**: capacity, totals, and penalties. `Item.Encumbrance` is an integer per item, but there is no aggregator. Tracked as TODO in `app-decisions.md`.

## Armor

- `Armor.AcBonus` — flat AC bonus when equipped (non-shield).
- `Armor.IsShield` — shields are armor entries with the shield flag.
- Only **one** non-shield armor and **one** shield can be equipped at a time (the codebase fetches "first" matching equipped armor — invariant should be maintained via UI).
- Shield grants **+1 AC** only when body armor is also equipped and the equipped weapon is **not** two-handed (see `combat.md`).
- Heavy armor traditionally restricts magic in WWN (mages cannot cast in armor heavier than light without penalty). **Not enforced** in code.

## Weapons

- `Weapon` extends `Item` and adds `DamageDie`, `AttributeModifier`, `CombatSkill`, `Tags`, optional `Shock`.
- Two-handed weapons cancel the shield AC bonus while equipped (see `combat.md`).

## App implications

- Slot moves go through `Character.EquipItem / UnequipItem / ReadyItem / ChangeItemSlot` (`src/WWN.Domain/Aggregates/Character.cs:233`).
- "First equipped armor / shield" lookup pattern means **the UI must enforce uniqueness** — the domain does not currently throw if two body armors are equipped.

## Data model implications

- Adding capacity tracking: derive `MaxReadied = STR_score / 2` (round down) and `MaxStowed = STR_score`. Sum item encumbrance per slot. Don't persist; recompute.
- Armor weight class (light/medium/heavy) is not modeled — would need a `WeightClass` enum on `Armor` for spell-failure rules.
- Currency / coin is not modeled. If added, treat as either a separate `Wealth` value object on `Character` or as a special inventory item.

## UI implications

- Inventory panel currently lists items by slot. Add: total encumbrance per slot, max capacity, over-cap warning.
- Equip toggle on weapons should warn if it would invalidate the shield bonus.
- Armor must distinguish shields visually.

## Open questions / ambiguities

- Exact encumbrance penalty bands (movement reduction thresholds) — confirm before encoding.
- Bulk/heavy item slot counting (e.g., a polearm = 2 readied slots) is not represented; `Encumbrance` is a per-item integer that the UI will probably need to interpret as "slots used".
- Armor weight class & mage-in-armor penalty — not modeled.
