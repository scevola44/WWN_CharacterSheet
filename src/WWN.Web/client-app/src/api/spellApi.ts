import axios from 'axios';
import type { Spell, KnownSpell, CreateSpellRequest, UseSpellSlotRequest } from '../types/spell';
import type { CharacterDetail } from '../types/character';

const spellApi = axios.create({ baseURL: '/api/spells' });
const charSpellApi = axios.create({ baseURL: '/api/characters' });

export const spellsApi = {
  // Spell catalog
  list: () => spellApi.get<Spell[]>('/').then(r => r.data),

  create: (req: CreateSpellRequest) =>
    spellApi.post<Spell>('/', req).then(r => r.data),

  delete: (id: string) => spellApi.delete(`/${id}`),

  // Character spells
  getCharacterSpells: (charId: string) =>
    charSpellApi.get<CharacterDetail>(`/${charId}/spells`).then(r => r.data),

  learnSpell: (charId: string, spellId: string) =>
    charSpellApi.post<KnownSpell>(`/${charId}/spells/${spellId}`).then(r => r.data),

  forgetSpell: (charId: string, spellId: string) =>
    charSpellApi.delete(`/${charId}/spells/${spellId}`),

  useSpellSlot: (charId: string, spellLevel: number) =>
    charSpellApi.post<CharacterDetail>(
      `/${charId}/spells/use-slot`,
      { spellLevel } as UseSpellSlotRequest
    ).then(r => r.data),

  restoreSpellSlots: (charId: string) =>
    charSpellApi.post<CharacterDetail>(`/${charId}/spells/restore-slots`).then(r => r.data),
};
