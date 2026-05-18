import { useState } from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { authApi } from '../api/authApi';
import { EMAIL_NOT_CONFIRMED_ERROR } from '../types/auth';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [needsConfirmation, setNeedsConfirmation] = useState(false);
  const [resendStatus, setResendStatus] = useState<'idle' | 'sending' | 'sent'>('idle');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const successMessage = (location.state as { message?: string } | null)?.message;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setNeedsConfirmation(false);
    setResendStatus('idle');
    setIsSubmitting(true);
    try {
      await login({ email, password });
      navigate('/');
    } catch (err: unknown) {
      const response = (err as { response?: { status?: number; data?: { error?: string; message?: string } } }).response;
      if (response?.status === 403 && response.data?.error === EMAIL_NOT_CONFIRMED_ERROR) {
        setNeedsConfirmation(true);
        setError(response.data?.message ?? 'Please confirm your email before signing in.');
      } else {
        setError('Invalid email or password.');
      }
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleResend = async () => {
    setResendStatus('sending');
    try {
      await authApi.resendConfirmation({ email });
    } finally {
      setResendStatus('sent');
    }
  };

  return (
    <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
      <h2>Sign In</h2>
      {successMessage && (
        <div style={{ color: 'green', marginBottom: '1rem' }}>{successMessage}</div>
      )}
      <form onSubmit={handleSubmit}>
        <div className="form-group" style={{ marginBottom: '1rem' }}>
          <label htmlFor="email">Email</label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            required
            style={{ display: 'block', width: '100%', marginTop: '0.25rem' }}
          />
        </div>
        <div className="form-group" style={{ marginBottom: '1rem' }}>
          <label htmlFor="password">Password</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
            style={{ display: 'block', width: '100%', marginTop: '0.25rem' }}
          />
        </div>
        {error && <div style={{ color: 'red', marginBottom: '1rem' }}>{error}</div>}
        {needsConfirmation && (
          <div style={{ marginBottom: '1rem' }}>
            {resendStatus === 'sent' ? (
              <div style={{ color: 'green' }}>
                If an account with that email exists, a confirmation email has been sent.
              </div>
            ) : (
              <button
                type="button"
                onClick={handleResend}
                disabled={resendStatus === 'sending' || !email}
                style={{ width: '100%' }}
              >
                {resendStatus === 'sending' ? 'Sending…' : 'Resend confirmation email'}
              </button>
            )}
          </div>
        )}
        <button type="submit" disabled={isSubmitting} style={{ width: '100%' }}>
          {isSubmitting ? 'Signing in...' : 'Sign In'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', textAlign: 'center' }}>
        <Link to="/forgot-password">Forgot password?</Link>
      </p>
      <p style={{ marginTop: '0.5rem', textAlign: 'center' }}>
        No account? <Link to="/register">Register</Link>
      </p>
    </div>
  );
}
