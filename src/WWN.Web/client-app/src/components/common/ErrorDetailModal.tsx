import { useEffect, useState } from 'react';
import type { ApiError } from '../../api/errorDispatcher';

export function ErrorDetailModal() {
  const [error, setError] = useState<ApiError | null>(null);
  const [copied, setCopied] = useState(false);

  useEffect(() => {
    const handler = (e: Event) => setError((e as CustomEvent<ApiError>).detail);
    window.addEventListener('api-error', handler);
    return () => window.removeEventListener('api-error', handler);
  }, []);

  if (!import.meta.env.DEV || !error) return null;

  const copyTraceId = () => {
    if (!error.traceId) return;
    navigator.clipboard.writeText(error.traceId);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div
      style={{
        position: 'fixed',
        inset: 0,
        background: 'rgba(0,0,0,0.75)',
        zIndex: 9999,
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '1rem',
      }}
      onClick={() => setError(null)}
    >
      <div
        style={{
          background: 'var(--surface, #1e1e2e)',
          border: '1px solid var(--danger, #f38ba8)',
          borderRadius: '8px',
          width: '100%',
          maxWidth: '600px',
          maxHeight: '80vh',
          overflow: 'auto',
          padding: '1.5rem',
        }}
        onClick={e => e.stopPropagation()}
      >
        {/* Header */}
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', marginBottom: '1rem' }}>
          <span
            style={{
              background: 'var(--danger, #f38ba8)',
              color: '#000',
              fontSize: '0.7rem',
              fontWeight: 700,
              letterSpacing: '0.05em',
              padding: '2px 8px',
              borderRadius: '4px',
            }}
          >
            DEV ERROR
          </span>
          <span
            style={{
              background: 'rgba(255,255,255,0.1)',
              color: 'var(--text, #cdd6f4)',
              fontSize: '0.8rem',
              fontWeight: 600,
              padding: '2px 8px',
              borderRadius: '4px',
              fontFamily: 'monospace',
            }}
          >
            HTTP {error.status}
          </span>
          <button
            onClick={() => setError(null)}
            style={{
              marginLeft: 'auto',
              background: 'none',
              border: 'none',
              color: 'var(--text-muted, #a6adc8)',
              fontSize: '1.25rem',
              cursor: 'pointer',
              lineHeight: 1,
              padding: '0 4px',
            }}
          >
            ✕
          </button>
        </div>

        {/* Title */}
        <p style={{ fontWeight: 700, color: 'var(--danger, #f38ba8)', margin: '0 0 0.5rem' }}>
          {error.title}
        </p>

        {/* Detail */}
        <p style={{ color: 'var(--text, #cdd6f4)', margin: '0 0 1rem', lineHeight: 1.5 }}>
          {error.detail}
        </p>

        {/* Trace ID */}
        {error.traceId && (
          <div style={{ marginBottom: '1rem' }}>
            <span style={{ fontSize: '0.75rem', color: 'var(--text-muted, #a6adc8)', marginRight: '0.5rem' }}>
              Trace ID
            </span>
            <code
              onClick={copyTraceId}
              title="Click to copy"
              style={{
                fontFamily: 'monospace',
                fontSize: '0.8rem',
                color: 'var(--accent, #89b4fa)',
                cursor: 'pointer',
                borderBottom: '1px dashed currentColor',
              }}
            >
              {error.traceId}
            </code>
            {copied && (
              <span style={{ fontSize: '0.75rem', color: 'var(--success, #a6e3a1)', marginLeft: '0.5rem' }}>
                copied
              </span>
            )}
          </div>
        )}

        {/* Stack trace */}
        {error.stackTrace && (
          <details>
            <summary
              style={{
                cursor: 'pointer',
                fontSize: '0.8rem',
                color: 'var(--text-muted, #a6adc8)',
                marginBottom: '0.5rem',
                userSelect: 'none',
              }}
            >
              Stack trace
            </summary>
            <pre
              style={{
                margin: 0,
                padding: '0.75rem',
                background: 'rgba(0,0,0,0.3)',
                borderRadius: '4px',
                fontSize: '0.7rem',
                color: 'var(--text-muted, #a6adc8)',
                overflowX: 'auto',
                whiteSpace: 'pre-wrap',
                wordBreak: 'break-word',
              }}
            >
              {error.stackTrace}
            </pre>
          </details>
        )}
      </div>
    </div>
  );
}
