import axios from 'axios';

const http = axios.create({ baseURL: '/api/diagnostics' });

export interface AppInfo {
  branch: string;
  environment: string;
  startupTime: string;
  version: string;
  channel: 'stable' | 'beta';
}

export const diagnosticsApi = {
  getInfo: () => http.get<AppInfo>('/info').then(r => r.data),
};
