import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';

export function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const userId = searchParams.get('userId') ?? '';
  const token = searchParams.get('token') ?? '';

  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const linkValid = userId !== '' && token !== '';

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (newPassword !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    setIsSubmitting(true);
    try {
      await authApi.resetPassword({ userId, token, newPassword });
      navigate('/login', { state: { message: 'Password reset. You can now sign in with your new password.' } });
    } catch (err: unknown) {
      const data = (err as { response?: { data?: { message?: string; errors?: string[] } } }).response?.data;
      setError(data?.errors?.join(' ') ?? data?.message ?? 'Password reset failed. The link may have expired.');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!linkValid) {
    return (
      <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
        <h2>Reset Password</h2>
        <p style={{ color: 'red' }}>This password reset link is invalid.</p>
        <p style={{ marginTop: '1rem', textAlign: 'center' }}>
          <Link to="/forgot-password">Request a new link</Link>
        </p>
      </div>
    );
  }

  return (
    <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
      <h2>Reset Password</h2>
      <form onSubmit={handleSubmit}>
        <div className="form-group" style={{ marginBottom: '1rem' }}>
          <label htmlFor="newPassword">New password (min 6 characters)</label>
          <input
            id="newPassword"
            type="password"
            value={newPassword}
            onChange={e => setNewPassword(e.target.value)}
            required
            minLength={6}
            style={{ display: 'block', width: '100%', marginTop: '0.25rem' }}
          />
        </div>
        <div className="form-group" style={{ marginBottom: '1rem' }}>
          <label htmlFor="confirmPassword">Confirm new password</label>
          <input
            id="confirmPassword"
            type="password"
            value={confirmPassword}
            onChange={e => setConfirmPassword(e.target.value)}
            required
            style={{ display: 'block', width: '100%', marginTop: '0.25rem' }}
          />
        </div>
        {error && <div style={{ color: 'red', marginBottom: '1rem' }}>{error}</div>}
        <button type="submit" disabled={isSubmitting} style={{ width: '100%' }}>
          {isSubmitting ? 'Resetting…' : 'Reset password'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', textAlign: 'center' }}>
        <Link to="/login">Back to sign in</Link>
      </p>
    </div>
  );
}
