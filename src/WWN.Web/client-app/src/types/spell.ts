export interface Spell {
  id: string;
  name: string;
  spellLevel: number;
  description: string;
  summary: string | null;
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
  summary?: string;
}

export interface UpdateSpellRequest {
  name: string;
  spellLevel: number;
  description: string;
  summary?: string;
}

export interface UseSpellSlotRequest {
  spellLevel: number;
}
