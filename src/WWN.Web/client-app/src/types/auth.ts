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
}

export interface LoginResponse {
  token: string;
}
