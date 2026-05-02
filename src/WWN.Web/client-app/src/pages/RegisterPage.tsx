import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function RegisterPage() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (password !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }
    setIsSubmitting(true);
    try {
      await register({ email, password });
      navigate('/login', { state: { message: 'Account created! Please sign in.' } });
    } catch (err: unknown) {
      const data = (err as { response?: { data?: { errors?: string[] } } }).response?.data;
      setError(data?.errors?.join(' ') ?? 'Registration failed. Please try again.');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
      <h2>Create Account</h2>
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
          <label htmlFor="password">Password (min 6 characters)</label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            required
            minLength={6}
            style={{ display: 'block', width: '100%', marginTop: '0.25rem' }}
          />
        </div>
        <div className="form-group" style={{ marginBottom: '1rem' }}>
          <label htmlFor="confirmPassword">Confirm Password</label>
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
          {isSubmitting ? 'Creating account...' : 'Create Account'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', textAlign: 'center' }}>
        Already have an account? <Link to="/login">Sign In</Link>
      </p>
    </div>
  );
}
