import { useState } from 'react';
import type { Art, UpdateArtRequest } from '../../types/art';
import { artsApi } from '../../api/artApi';
import { ArtForm } from './ArtForm';
import { useEffortCommitment } from '../../contexts/LookupsContext';

export function ArtDetailModal({ art, onClose, onSaved }: {
  art: Art;
  onClose: () => void;
  onSaved: (updated: Art) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [editForm, setEditForm] = useState<UpdateArtRequest>({
    name: art.name,
    description: art.description,
    summary: art.summary ?? '',
    minLevel: art.minLevel,
    effortCost: art.effortCost,
    source: art.source,
  });
  const effort = useEffortCommitment(art.effortCost);

  const handleSave = async () => {
    const updated = await artsApi.update(art.id, editForm);
    onSaved(updated);
    setEditing(false);
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: 600, width: '100%', maxHeight: '80vh', overflowY: 'auto', margin: '1rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>{art.name}</h2>
          <button className="sm secondary" onClick={onClose}>✕</button>
        </div>

        {editing ? (
          <ArtForm
            values={editForm}
            onChange={setEditForm}
            onSubmit={handleSave}
            onCancel={() => setEditing(false)}
            submitLabel="Save"
          />
        ) : (
          <>
            <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginBottom: '0.75rem' }}>
              Min Level {art.minLevel} · Effort: {effort?.displayName ?? '—'} · {art.source}
            </div>

            {art.summary && (
              <p style={{ margin: '0 0 0.75rem 0', fontStyle: 'italic', color: 'var(--text-muted)' }}>{art.summary}</p>
            )}

            <p style={{ margin: '0 0 1.25rem 0', fontSize: '0.9rem' }}>{art.description}</p>

            <button className="secondary" onClick={() => setEditing(true)}>Edit</button>
          </>
        )}
      </div>
    </div>
  );
}
