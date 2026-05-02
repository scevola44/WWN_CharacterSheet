import type { AxiosInstance } from 'axios';
import { dispatchApiError } from './errorDispatcher';

const TOKEN_KEY = 'wwn_auth_token';

// Module-level token store — updated by AuthContext on login/logout
let _token: string | null = null;

export function setAuthToken(token: string | null) {
  _token = token;
}

export function applyDevErrorInterceptor(instance: AxiosInstance) {
  // Attach Bearer token to every request
  instance.interceptors.request.use(config => {
    if (_token) {
      config.headers.Authorization = `Bearer ${_token}`;
    }
    return config;
  });

  instance.interceptors.response.use(
    r => r,
    err => {
      const status = err.response?.status;

      if (status === 401) {
        // Token expired or invalid — clear and redirect to login
        setAuthToken(null);
        localStorage.removeItem(TOKEN_KEY);
        window.location.href = '/login';
        return Promise.reject(err);
      }

      if (import.meta.env.DEV) {
        const data = err.response?.data;
        dispatchApiError({
          status: status ?? 0,
          title: data?.title ?? 'Error',
          detail: data?.detail ?? err.message,
          traceId: data?.traceId,
          stackTrace: data?.stackTrace,
        });
      }

      return Promise.reject(err);
    }
  );
}
