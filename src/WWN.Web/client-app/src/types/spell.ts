export interface Spell {
  id: string;
  name: string;
  spellLevel: number;
  description: string;
  school: string | null;
  duration: string | null;
  range: string | null;
}

export interface KnownSpell {
  id: string;
  spellId: string;
  spell: Spell;
}

export interface CreateSpellRequest {
  name: string;
  spellLevel: number;
  description: string;
  school?: string;
  duration?: string;
  range?: string;
}

export interface SpellSlots {
  available: number[];
  used: number[];
}

export interface UseSpellSlotRequest {
  spellLevel: number;
}
