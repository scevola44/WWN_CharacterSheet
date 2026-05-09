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
