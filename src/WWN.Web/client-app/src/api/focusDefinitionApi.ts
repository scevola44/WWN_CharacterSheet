import axios from 'axios';
import type {
  FocusDefinition,
  CreateFocusDefinitionRequest,
  UpdateFocusDefinitionRequest,
} from '../types/focusDefinition';

const api = axios.create({ baseURL: '/api/focus-definitions' });

export const focusDefinitionApi = {
  list: () => api.get<FocusDefinition[]>('/').then(r => r.data),

  get: (id: string) => api.get<FocusDefinition>(`/${id}`).then(r => r.data),

  create: (req: CreateFocusDefinitionRequest) =>
    api.post<FocusDefinition>('/', req).then(r => r.data),

  update: (id: string, req: UpdateFocusDefinitionRequest) =>
    api.put<FocusDefinition>(`/${id}`, req).then(r => r.data),

  delete: (id: string) => api.delete(`/${id}`),
};
