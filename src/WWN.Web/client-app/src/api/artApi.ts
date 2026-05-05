import axios from 'axios';
import type {
  Art,
  KnownArt,
  CreateArtRequest,
  UpdateArtRequest,
  EffortCommitment,
} from '../types/art';
import type { CharacterDetail } from '../types/character';
import { applyDevErrorInterceptor } from './apiInterceptor';

const artApi = axios.create({ baseURL: '/api/arts' });
applyDevErrorInterceptor(artApi);
const charArtApi = axios.create({ baseURL: '/api/characters' });
applyDevErrorInterceptor(charArtApi);

export const artsApi = {
  list: () => artApi.get<Art[]>('/').then(r => r.data),

  get: (id: string) => artApi.get<Art>(`/${id}`).then(r => r.data),

  create: (req: CreateArtRequest) =>
    artApi.post<Art>('/', req).then(r => r.data),

  update: (id: string, req: UpdateArtRequest) =>
    artApi.put<Art>(`/${id}`, req).then(r => r.data),

  delete: (id: string) => artApi.delete(`/${id}`),

  learnArt: (charId: string, artId: string) =>
    charArtApi.post<KnownArt>(`/${charId}/arts/${artId}`).then(r => r.data),

  forgetArt: (charId: string, artId: string) =>
    charArtApi.delete(`/${charId}/arts/${artId}`),

  commitEffort: (charId: string, commitment: EffortCommitment, amount = 1) =>
    charArtApi.post<CharacterDetail>(`/${charId}/arts/commit-effort`, { commitment, amount })
      .then(r => r.data),

  endScene: (charId: string) =>
    charArtApi.post<CharacterDetail>(`/${charId}/arts/end-scene`).then(r => r.data),

  restDay: (charId: string) =>
    charArtApi.post<CharacterDetail>(`/${charId}/arts/rest-day`).then(r => r.data),

  releaseSustained: (charId: string, amount = 1) =>
    charArtApi.post<CharacterDetail>(`/${charId}/arts/release-sustained`, { amount })
      .then(r => r.data),
};
