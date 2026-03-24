import axios from 'axios';
import type {
  CharacterSummary,
  CharacterDetail,
  CreateCharacterRequest,
  AddItemRequest,
  AddFocusRequest,
} from '../types/character';
import { applyDevErrorInterceptor } from './apiInterceptor';

const api = axios.create({ baseURL: '/api/characters' });
applyDevErrorInterceptor(api);

export const characterApi = {
  list: () => api.get<CharacterSummary[]>('/').then(r => r.data),

  get: (id: string) => api.get<CharacterDetail>(`/${id}`).then(r => r.data),

  create: (req: CreateCharacterRequest) =>
    api.post<{ id: string }>('/', req).then(r => r.data.id),

  delete: (id: string) => api.delete(`/${id}`),

  updateAttribute: (id: string, attr: string, score: number) =>
    api.put<CharacterDetail>(`/${id}/attributes/${attr}`, { score }).then(r => r.data),

  updateSkill: (id: string, skill: string, level: number) =>
    api.put<CharacterDetail>(`/${id}/skills/${skill}`, { level }).then(r => r.data),

  addCustomSkill: (id: string, name: string, level: number) =>
    api.post<CharacterDetail>(`/${id}/skills/custom`, { name, level }).then(r => r.data),

  setHp: (id: string, maxHitPoints: number, currentHitPoints: number) =>
    api.put<CharacterDetail>(`/${id}/hp`, { maxHitPoints, currentHitPoints }).then(r => r.data),

  setLevel: (id: string, level: number) =>
    api.put<CharacterDetail>(`/${id}/level`, { level }).then(r => r.data),

  addFocus: (id: string, req: AddFocusRequest) =>
    api.post<CharacterDetail>(`/${id}/foci`, req).then(r => r.data),

  removeFocus: (id: string, focusId: string) =>
    api.delete(`/${id}/foci/${focusId}`),

  addItem: (id: string, req: AddItemRequest) =>
    api.post<CharacterDetail>(`/${id}/items`, req).then(r => r.data),

  removeItem: (id: string, itemId: string) =>
    api.delete(`/${id}/items/${itemId}`),

  updateItem: (id: string, itemId: string, req: AddItemRequest) =>
    api.put<CharacterDetail>(`/${id}/items/${itemId}`, req).then(r => r.data),

  changeSlot: (id: string, itemId: string, slotType: string) =>
    api.put<CharacterDetail>(`/${id}/items/${itemId}/slot`, { slotType }).then(r => r.data),

  updateWeaponAttackConfig: (id: string, itemId: string, skill: string, attribute: string) =>
    api.put<CharacterDetail>(`/${id}/items/${itemId}/attack-config`, { skill, attribute }).then(r => r.data),

  updateNotes: (id: string, notes: string | null) =>
    api.put<CharacterDetail>(`/${id}/notes`, { notes }).then(r => r.data),
};
