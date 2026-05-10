import { useState, useEffect } from 'react';
import { adminApi } from '../api/adminApi';
import { diagnosticsApi, type AppInfo } from '../api/diagnosticsApi';
import { useAuth } from '../contexts/AuthContext';

export function AdminPage() {
  const { token } = useAuth();
  const [status, setStatus] = useState<{ ok: boolean; message: string } | null>(null);
  const [loading, setLoading] = useState(false);

  const [appInfo, setAppInfo] = useState<AppInfo | null>(null);
  const [infoError, setInfoError] = useState(false);

  useEffect(() => {
    diagnosticsApi.getInfo()
      .then(setAppInfo)
      .catch(() => setInfoError(true));
  }, []);

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
        <h3>App Version</h3>
        {infoError && (
          <p style={{ color: 'var(--color-error, red)' }}>Could not load version info.</p>
        )}
        {!infoError && !appInfo && (
          <p style={{ color: 'var(--text-muted)' }}>Loading…</p>
        )}
        {appInfo && (
          <p style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <span style={{ fontWeight: 'bold', fontFamily: 'monospace', fontSize: '1.1rem' }}>
              {appInfo.version}
            </span>
            <span style={{
              display: 'inline-block',
              padding: '0.15rem 0.5rem',
              borderRadius: '999px',
              fontSize: '0.75rem',
              fontWeight: 'bold',
              textTransform: 'uppercase',
              background: appInfo.channel === 'stable' ? 'var(--success)' : 'var(--warning)',
              color: 'white',
            }}>
              {appInfo.channel}
            </span>
          </p>
        )}
      </section>

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
