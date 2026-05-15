import { useState } from 'react';
import { Link } from 'react-router-dom';
import { authApi } from '../api/authApi';

export function ForgotPasswordPage() {
  const [email, setEmail] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitted, setSubmitted] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    try {
      await authApi.forgotPassword({ email });
    } finally {
      setIsSubmitting(false);
      setSubmitted(true);
    }
  };

  if (submitted) {
    return (
      <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
        <h2>Check your email</h2>
        <p>
          If an account with that email exists, we've sent a password reset link. Please check your inbox.
        </p>
        <p style={{ marginTop: '1rem', textAlign: 'center' }}>
          <Link to="/login">Back to sign in</Link>
        </p>
      </div>
    );
  }

  return (
    <div className="page-content" style={{ maxWidth: 400, margin: '4rem auto', padding: '0 1rem' }}>
      <h2>Forgot Password</h2>
      <p>Enter the email address associated with your account and we'll send you a link to reset your password.</p>
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
        <button type="submit" disabled={isSubmitting} style={{ width: '100%' }}>
          {isSubmitting ? 'Sending…' : 'Send reset link'}
        </button>
      </form>
      <p style={{ marginTop: '1rem', textAlign: 'center' }}>
        <Link to="/login">Back to sign in</Link>
      </p>
    </div>
  );
}
