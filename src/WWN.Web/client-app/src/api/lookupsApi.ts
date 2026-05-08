import axios from 'axios';
import type { Lookups } from '../types/lookups';
import { applyDevErrorInterceptor } from './apiInterceptor';

const lookupsHttp = axios.create({ baseURL: '/api' });
applyDevErrorInterceptor(lookupsHttp);

export const lookupsApi = {
  getAll: () => lookupsHttp.get<Lookups>('/lookups').then(r => r.data),
};
