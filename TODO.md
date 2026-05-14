# Feature Implementation Roadmap

## Medium Priority
- [ ] Attack roll bonus breakdown (show formula: attr mod + skill mod + BAB + item bonuses)
- [ ] Initiative mechanics (calculate DEX-based initiative bonus, surface in combat panel)

## Low Priority
- [ ] Proficiency/Skill Point economy (track available vs spent, cost-per-rank table, Expert class grants)
- [ ] Wealth/treasure tracking (separate money inventory, hireling payment tracking)
- [ ] HP recovery on rest (pick a rule: full heal? level×1 HP? fraction of max?)
- [ ] Death and dying state (mortal-wound timer, stabilization mechanic)
- [ ] Heavy armor / spell failure for mages (requires ArmorWeightClass enum)
- [ ] Status conditions (poisoned, prone, frightened — as typed StatusCondition list on Character)
- [ ] Enhanced shock damage display (more prominent threshold visibility)

## Completed
- [x] Character creation and management
- [x] Attributes and modifiers
- [x] 21 base skills + custom skill support (ranks −1..4)
- [x] Combat stats (AC, BAB, saves — Physical, Evasion, Mental, Luck)
- [x] Weapon and armor management
- [x] Class abilities with level filtering
- [x] Foci system with conditional effects
- [x] Spellcasting and spell slots (6 levels, usage tracking, learn/forget)
- [x] Arts & Effort system (pool calc, Scene/Day/Sustained commitment, End Scene / Rest for Day)
- [x] Hit point tracker
- [x] System Strain tracking (current/max = CON score, manual +/−, max-strain warning)
- [x] Total encumbrance display (readied load vs ½ STR, stowed load vs STR, over-cap warning)
- [x] Item notes
