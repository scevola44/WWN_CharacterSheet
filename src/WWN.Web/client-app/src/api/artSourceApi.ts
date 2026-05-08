import axios from 'axios';
import type { LookupValue } from '../types/lookups';
import { applyDevErrorInterceptor } from './apiInterceptor';

export interface CreateArtSourceRequest {
  code: string;
  displayName: string;
  description?: string;
  sortOrder: number;
}

export interface UpdateArtSourceRequest {
  displayName: string;
  description?: string;
  sortOrder: number;
}

const api = axios.create({ baseURL: '/api/art-sources' });
applyDevErrorInterceptor(api);

export const artSourceApi = {
  list: () => api.get<LookupValue[]>('/').then(r => r.data),

  create: (req: CreateArtSourceRequest) =>
    api.post<LookupValue>('/', req).then(r => r.data),

  update: (id: number, req: UpdateArtSourceRequest) =>
    api.put<LookupValue>(`/${id}`, req).then(r => r.data),

  delete: (id: number) => api.delete(`/${id}`),
};
