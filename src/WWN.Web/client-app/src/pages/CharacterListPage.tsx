import { useNavigate } from 'react-router-dom';
import { useCharacterList } from '../hooks/useCharacterList';
import { characterApi } from '../api/characterApi';

export function CharacterListPage() {
  const { characters, loading, refresh } = useCharacterList();
  const navigate = useNavigate();

  const handleDelete = async (e: React.MouseEvent, id: string) => {
    e.stopPropagation();
    if (confirm('Delete this character?')) {
      await characterApi.delete(id);
      refresh();
    }
  };

  if (loading) return <div className="loading">Loading characters...</div>;

  return (
    <div>
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
                onClick={e => handleDelete(e, c.id)}>
                Delete
              </button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
