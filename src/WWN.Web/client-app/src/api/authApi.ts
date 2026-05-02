import axios from 'axios';
import type { LoginRequest, RegisterRequest, LoginResponse, AuthUser } from '../types/auth';

// Separate instance without auth interceptor to avoid circular dependency with AuthContext
const authHttp = axios.create({ baseURL: '/api/auth' });

export const authApi = {
  login: (req: LoginRequest) =>
    authHttp.post<LoginResponse>('/login', req).then(r => r.data),

  register: (req: RegisterRequest) =>
    authHttp.post<{ message: string }>('/register', req).then(r => r.data),

  me: (token: string) =>
    authHttp.get<AuthUser>('/me', {
      headers: { Authorization: `Bearer ${token}` }
    }).then(r => r.data),
};
