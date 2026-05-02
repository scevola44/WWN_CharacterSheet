import { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from 'react';
import { authApi } from '../api/authApi';
import { setAuthToken } from '../api/apiInterceptor';
import type { AuthUser, LoginRequest, RegisterRequest } from '../types/auth';

const TOKEN_KEY = 'wwn_auth_token';

interface AuthContextValue {
  user: AuthUser | null;
  token: string | null;
  isLoading: boolean;
  login: (req: LoginRequest) => Promise<void>;
  register: (req: RegisterRequest) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [isLoading, setIsLoading] = useState(true);

  // Sync token to axios interceptor whenever it changes
  useEffect(() => {
    setAuthToken(token);
  }, [token]);

  // On mount, validate any stored token by calling /me
  useEffect(() => {
    const storedToken = localStorage.getItem(TOKEN_KEY);
    if (!storedToken) {
      setIsLoading(false);
      return;
    }

    authApi.me(storedToken)
      .then(authUser => {
        setUser(authUser);
        setToken(storedToken);
      })
      .catch(() => {
        localStorage.removeItem(TOKEN_KEY);
        setToken(null);
        setUser(null);
      })
      .finally(() => setIsLoading(false));
  }, []);

  const login = useCallback(async (req: LoginRequest) => {
    const { token: newToken } = await authApi.login(req);
    localStorage.setItem(TOKEN_KEY, newToken);
    setToken(newToken);
    const authUser = await authApi.me(newToken);
    setUser(authUser);
  }, []);

  const register = useCallback(async (req: RegisterRequest) => {
    await authApi.register(req);
    // Registration does not auto-login — user must sign in after registering
  }, []);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    setToken(null);
    setUser(null);
  }, []);

  return (
    <AuthContext.Provider value={{ user, token, isLoading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider');
  return ctx;
}
