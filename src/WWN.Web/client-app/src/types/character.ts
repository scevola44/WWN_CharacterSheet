export interface CharacterSummary {
  id: string;
  name: string;
  class: string;
  level: number;
  currentHitPoints: number;
  maxHitPoints: number;
}

export interface CharacterDetail {
  id: string;
  name: string;
  background: string | null;
  origin: string | null;
  class: string;
  partialClassA: string | null;
  partialClassB: string | null;
  level: number;
  maxHitPoints: number;
  currentHitPoints: number;
  experiencePoints: number;
  attributes: AttributeInfo[];
  skills: SkillInfo[];
  foci: FocusInfo[];
  inventory: ItemInfo[];
  spellbook: KnownSpellInfo[];
  spellSlots: SpellSlotInfo | null;
  derivedStats: DerivedStats;
  notes: string | null;
}

export interface AttributeInfo {
  name: string;
  score: number;
  modifier: number;
}

export interface SkillInfo {
  id: string;
  name: string;
  customName: string | null;
  level: number;
}

export interface FocusInfo {
  id: string;
  name: string;
  level: number;
  effects: FocusEffectInfo[];
}

export interface FocusEffectInfo {
  type: string;
  numericValue: number;
  targetSkill: string | null;
  targetAttribute: string | null;
  description: string | null;
}

export interface ItemInfo {
  id: string;
  name: string;
  description: string | null;
  encumbrance: number;
  slotType: string;
  quantity: number;
  itemType: string;
  damageDie: string | null;
  attributeModifier: string | null;
  shockDamage: number | null;
  shockAcThreshold: number | null;
  tags: string | null;
  attackBonus: number | null;
  acBonus: number | null;
  isShield: boolean | null;
}

export interface DerivedStats {
  armorClass: number;
  baseAttackBonus: number;
  physicalSave: number;
  evasionSave: number;
  mentalSave: number;
  attributeModifiers: Record<string, number>;
  weaponAttackBonuses: Record<string, number>;
  hitDieModifier: number;
}

export interface CreateCharacterRequest {
  name: string;
  background?: string;
  origin?: string;
  class: string;
  partialClassA?: string;
  partialClassB?: string;
  attributes: Record<string, number>;
  maxHitPoints?: number;
}

export interface AddItemRequest {
  name: string;
  description?: string;
  encumbrance: number;
  slotType?: string;
  quantity?: number;
  itemType: string;
  damageDieCount?: number;
  damageDieSides?: number;
  attributeModifier?: string;
  shockDamage?: number;
  shockAcThreshold?: number;
  tags?: string;
  acBonus?: number;
  isShield?: boolean;
}

export interface AddFocusRequest {
  name: string;
  level: number;
  effects: {
    type: string;
    numericValue: number;
    targetSkill?: string;
    targetAttribute?: string;
    description?: string;
  }[];
}

export interface KnownSpellInfo {
  id: string;
  spellId: string;
  spell: SpellInfo;
}

export interface SpellInfo {
  id: string;
  name: string;
  spellLevel: number;
  description: string;
  school: string | null;
  duration: string | null;
  range: string | null;
}

export interface SpellSlotInfo {
  available: number[];
  used: number[];
}
