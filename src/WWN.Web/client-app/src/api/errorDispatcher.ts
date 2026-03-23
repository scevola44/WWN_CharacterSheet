export type ApiError = {
  status: number;
  title: string;
  detail: string;
  traceId?: string;
  stackTrace?: string;
};

export function dispatchApiError(error: ApiError) {
  window.dispatchEvent(new CustomEvent<ApiError>('api-error', { detail: error }));
}
