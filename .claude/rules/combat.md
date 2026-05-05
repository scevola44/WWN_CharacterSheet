# Combat

Source: free WWN rules, paraphrased.

## Attack roll formula

```
attack = 1d20 + BAB + combat_skill_rank + attribute_mod + focus_bonus + class_ability_bonus
hit if attack ≥ target_AC
```

- **Combat skill** is one of `Stab` (melee/thrown), `Shoot` (ranged), or `Punch` (unarmed). Stored on `Weapon.CombatSkill`; defaulted via `CombatCalculator.GetCombatSkillForWeapon` (Ranged ⇒ Shoot, else Stab).
- **Attribute** is also stored on the weapon (`Weapon.AttributeModifier`) and is configurable per weapon — many weapons can be used with STR or DEX.
- Untrained skill rank = −1 in this codebase. WWN's "no skill" penalty for combat skills is captured as the −1.

Implementation: `CombatCalculator.GetTotalAttackBonus`.

## Damage

```
damage = weapon_die + attribute_mod + focus_bonus + class_ability_bonus
```

`CombatCalculator.GetTotalDamageBonus` returns the bonus only; the die is rolled UI-side. Attribute is whatever the weapon has configured.

## Shock

WWN's signature mechanic: a melee weapon with a shock entry deals automatic damage to a target whose AC is **at or below** the weapon's shock threshold, **even on a miss**, provided the wielder is engaging in melee.

`Weapon.Shock` is a `ShockInfo` value object: `Damage` and an AC threshold. Total shock damage:

```
shock = weapon_shock_damage + focus_shock_bonus + ability_shock_bonus
```

Implementation: `CombatCalculator.GetTotalShockBonus`. Threshold logic itself (compare to target AC) belongs in the combat-resolver UI; the calculator just totals the bonus.

## Armor Class

```
AC = 10 + armor_AC_bonus + DEX_mod + shield_bonus + focus_AC + ability_AC
```

- `armor_AC_bonus` from worn armor (non-shield equipped armor item).
- `shield_bonus = +1` only when **all** of: an `Armor` shield is equipped, body armor is equipped, **and** the equipped weapon is **not** two-handed.
- DEX mod always applies (the codebase does not currently model armor DEX caps).

Implementation: `CombatCalculator.GetArmorClass`.

## Weapon tags

Stored as `[Flags]` (`WeaponTag` enum). Bits:

| Tag | Meaning |
|-----|---------|
| Melee | Melee weapon |
| Ranged | Ranged weapon (sets default combat skill = Shoot) |
| TwoHanded | Two-handed; blocks shield bonus when equipped |
| Subtle | Concealable / surprise-friendly |
| Long | Reach |
| Thrown | Can be thrown (also melee in many cases) |
| AP | Armor-piercing (target's armor partly ignored — UI/resolver concern) |
| Reload | Requires reload action |

Tags are combinable with `|`. AP semantics (how much armor is ignored) are not modeled in calculators today — handled by the GM/UI.

## Initiative

Not implemented. WWN initiative is typically `1d8 + DEX mod` (lower can also be used per group rules), with class features modifying it. Tracked as a TODO; see `app-decisions.md` and the `Initiative` `FocusEffectType` value (already defined for future use).

## App implications

- The full attack pipeline is in `CombatCalculator`. Reuse, don't duplicate.
- AC, attack bonus, damage bonus, shock bonus are all functions of *current* equipment, attributes, foci, and class abilities. Recompute on read.
- When equipment changes (equip/unequip/ready), the recomputation happens for free because nothing is cached.

## Data model implications

- A weapon's combat skill and attribute can be reconfigured at runtime via `Weapon.SetCombatConfig` — useful when a player wants to use, say, a rapier with DEX vs STR.
- Shock is optional (`ShockInfo?`).
- AP is currently a tag only; if it ever becomes numeric (e.g., AP 1 / AP 2), promote it to a property or an `ApValue`.

## UI implications

- Weapon row should display: name, attack total, damage formula, shock (if present), tags, slot.
- Attack-bonus breakdown is a known UI gap — show `BAB + skill + attr + foci` as separate addends.
- Shield rules are subtle; when the equipped weapon becomes two-handed, the AC bar should drop by 1 with a tooltip explaining why.
- Initiative tracker is missing.

## Open questions / ambiguities

- AP value/threshold is not captured numerically.
- Armor with explicit DEX caps is not modeled.
- Initiative formula varies between GM tables; pick a default and let users override (TODO).
