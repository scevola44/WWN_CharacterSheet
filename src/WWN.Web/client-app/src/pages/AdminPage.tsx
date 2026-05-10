import { useState } from 'react';
import { adminApi } from '../api/adminApi';
import { useAuth } from '../contexts/AuthContext';

export function AdminPage() {
  const { token } = useAuth();
  const [status, setStatus] = useState<{ ok: boolean; message: string } | null>(null);
  const [loading, setLoading] = useState(false);

  const handleReloadLookups = async () => {
    setLoading(true);
    setStatus(null);
    try {
      const res = await adminApi.reloadLookups(token!);
      setStatus({ ok: true, message: res.message });
    } catch {
      setStatus({ ok: false, message: 'Failed to reload lookup tables.' });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-content">
      <h2>Admin</h2>

      <section>
        <h3>Lookup Tables</h3>
        <p>Force-reload all default lookup data (spells, foci, arts, class abilities) from the built-in seed data. Existing entries are deleted and recreated.</p>
        <button onClick={handleReloadLookups} disabled={loading}>
          {loading ? 'Reloading…' : 'Reload lookup tables'}
        </button>
        {status && (
          <p style={{ marginTop: '0.5rem', color: status.ok ? 'var(--color-success, green)' : 'var(--color-error, red)' }}>
            {status.message}
          </p>
        )}
      </section>
    </div>
  );
}
