import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useCharacterList } from '../hooks/useCharacterList';
import { characterApi } from '../api/characterApi';
import { ConfirmModal } from '../components/common/ConfirmModal';

export function CharacterListPage() {
  const { characters, loading, refresh } = useCharacterList();
  const navigate = useNavigate();
  const [pendingDelete, setPendingDelete] = useState<{ id: string; name: string } | null>(null);

  const handleDelete = (e: React.MouseEvent, id: string, name: string) => {
    e.stopPropagation();
    setPendingDelete({ id, name });
  };

  if (loading) return <div className="loading">Loading characters...</div>;

  return (
    <div className="page-content">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
        <h2>Characters</h2>
        <button onClick={() => navigate('/new')}>+ New Character</button>
      </div>

      {characters.length === 0 ? (
        <div className="loading">No characters yet. Create one to get started!</div>
      ) : (
        <div className="char-grid">
          {characters.map(c => (
            <div key={c.id} className="char-card" onClick={() => navigate(`/character/${c.id}`)}>
              <h3>{c.name}</h3>
              <div className="meta">
                Level {c.level} {c.class}
              </div>
              <div className="meta">
                HP: {c.currentHitPoints}/{c.maxHitPoints}
              </div>
              <button className="sm danger" style={{ marginTop: '0.5rem' }}
                onClick={e => handleDelete(e, c.id, c.name)}>
                Delete
              </button>
            </div>
          ))}
        </div>
      )}

      <ConfirmModal
        isOpen={pendingDelete !== null}
        title="Delete Character"
        message={<>Are you sure you want to permanently delete <strong>{pendingDelete?.name}</strong>? This cannot be undone.</>}
        confirmLabel="Delete Forever"
        onConfirm={async () => {
          await characterApi.delete(pendingDelete!.id);
          setPendingDelete(null);
          refresh();
        }}
        onCancel={() => setPendingDelete(null)}
      />
    </div>
  );
}
