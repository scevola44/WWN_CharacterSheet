import axios from 'axios';
import type {
  LoginRequest,
  RegisterRequest,
  LoginResponse,
  AuthUser,
  ConfirmEmailRequest,
  ResendConfirmationRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
} from '../types/auth';

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

  confirmEmail: (req: ConfirmEmailRequest) =>
    authHttp.post<{ message: string }>('/confirm-email', req).then(r => r.data),

  resendConfirmation: (req: ResendConfirmationRequest) =>
    authHttp.post<{ message: string }>('/resend-confirmation', req).then(r => r.data),

  forgotPassword: (req: ForgotPasswordRequest) =>
    authHttp.post<{ message: string }>('/forgot-password', req).then(r => r.data),

  resetPassword: (req: ResetPasswordRequest) =>
    authHttp.post<{ message: string }>('/reset-password', req).then(r => r.data),
};
