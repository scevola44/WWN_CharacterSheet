import { useEffect, useRef, useState } from 'react';
import { Link, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';

type Status = 'pending' | 'success' | 'error';

export function ConfirmEmailPage() {
  const [searchParams] = useSearchParams();
  const userId = searchParams.get('userId');
  const token = searchParams.get('token');
  const linkValid = !!userId && !!token;

  const [status, setStatus] = useState<Status>(linkValid ? 'pending' : 'error');
  const [message, setMessage] = useState(
    linkValid ? 'Confirming your email…' : 'Invalid confirmation link.'
  );
  const requestedRef = useRef(false);

  useEffect(() => {
    if (!linkValid || requestedRef.current) return;
    requestedRef.current = true;

    authApi.confirmEmail({ userId: userId!, token: token! })
      .then(res => {
        setStatus('success');
        setMessage(res.message ?? 'Email confirmed. You can now sign in.');
      })
      .catch((err: unknown) => {
        const data = (err as { response?: { data?: { message?: string } } }).response?.data;
        setStatus('error');
        setMessage(data?.message ?? 'We could not confirm your email. The link may have expired.');
      });
  }, [linkValid, userId, token]);

  const color = status === 'success' ? 'green' : status === 'error' ? 'red' : 'inherit';

  return (
    <div className="page-content" style={{ maxWidth: 480, margin: '4rem auto', padding: '0 1rem', textAlign: 'center' }}>
      <h2>Email Confirmation</h2>
      <p style={{ color, marginTop: '1.5rem' }}>{message}</p>
      {status === 'success' && (
        <p style={{ marginTop: '1.5rem' }}>
          <Link to="/login">Go to sign in</Link>
        </p>
      )}
      {status === 'error' && (
        <p style={{ marginTop: '1.5rem' }}>
          <Link to="/login">Back to sign in</Link>
        </p>
      )}
    </div>
  );
}
