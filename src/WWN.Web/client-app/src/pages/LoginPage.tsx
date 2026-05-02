import { useState } from 'react';
import { useNavigate, Link, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const successMessage = (location.state as { message?: string } | null)?.message;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      await login({ email, password });
      navigate('/');
    } catch {
      setError('Invalid email or password.');
    } finally {
      setIsSubmitting(false);
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
        <button type="submit" disabled={isSubmitting} style={{ width: '100%' }}>
          {isSubmitting ? 'Signing in...' : 'Sign In'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', textAlign: 'center' }}>
        No account? <Link to="/register">Register</Link>
      </p>
    </div>
  );
}
