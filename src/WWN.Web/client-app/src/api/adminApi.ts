import axios from 'axios';

const http = axios.create({ baseURL: '/api/admin' });

export const adminApi = {
  reloadLookups: (token: string) =>
    http.post<{ message: string }>('/reload-lookups', null, {
      headers: { Authorization: `Bearer ${token}` },
    }).then(r => r.data),
};
