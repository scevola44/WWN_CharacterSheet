export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
}

export interface AuthUser {
  userId: string;
  email: string;
  isAdmin: boolean;
}

export interface LoginResponse {
  token: string;
}

export interface ConfirmEmailRequest {
  userId: string;
  token: string;
}

export interface ResendConfirmationRequest {
  email: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  userId: string;
  token: string;
  newPassword: string;
}

export const EMAIL_NOT_CONFIRMED_ERROR = 'email_not_confirmed';
