import type { AxiosInstance } from 'axios';
import { dispatchApiError } from './errorDispatcher';

export function applyDevErrorInterceptor(instance: AxiosInstance) {
  if (!import.meta.env.DEV) return;
  instance.interceptors.response.use(
    r => r,
    err => {
      const data = err.response?.data;
      dispatchApiError({
        status: err.response?.status ?? 0,
        title: data?.title ?? 'Error',
        detail: data?.detail ?? err.message,
        traceId: data?.traceId,
        stackTrace: data?.stackTrace,
      });
      return Promise.reject(err);
    }
  );
}
