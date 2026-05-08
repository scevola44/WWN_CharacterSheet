import { useState, useEffect, useMemo } from 'react';
import type { Art } from '../../types/art';
import type { CharacterDetail } from '../../types/character';
import { artsApi } from '../../api/artApi';
import { useEffortCommitments, useLookups } from '../../contexts/LookupsContext';

const ALL_FILTER = -1;

export function ArtDatabaseModal({ character, onLearn, onClose }: {
  character: CharacterDetail;
  onLearn: () => void;
  onClose: () => void;
}) {
  const [arts, setArts] = useState<Art[]>([]);
  const [searchName, setSearchName] = useState('');
  const [filterEffort, setFilterEffort] = useState<number>(ALL_FILTER);
  const [loading, setLoading] = useState(true);
  const effortOptions = useEffortCommitments();
  const { effortCommitmentById } = useLookups();

  useEffect(() => {
    artsApi.list().then(setArts).finally(() => setLoading(false));
  }, []);

  const filteredArts = useMemo(() => {
    const knownArtIds = new Set(character.arts.map(a => a.artId));
    let filtered = arts.filter(a => !knownArtIds.has(a.id) && a.minLevel <= character.level);
    if (filterEffort !== ALL_FILTER) {
      filtered = filtered.filter(a => a.effortCost === filterEffort);
    }
    if (searchName) {
      filtered = filtered.filter(a => a.name.toLowerCase().includes(searchName.toLowerCase()));
    }
    return filtered;
  }, [arts, filterEffort, searchName, character.arts, character.level]);

  const handleLearn = async (artId: string) => {
    await artsApi.learnArt(character.id, artId);
    onLearn();
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: '600px', maxHeight: '80vh', overflow: 'auto', minWidth: '400px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2>Add Art</h2>
          <button className="sm danger" onClick={onClose}>✕</button>
        </div>

        <div className="form-group">
          <label>Search Name</label>
          <input
            type="text"
            value={searchName}
            onChange={e => setSearchName(e.target.value)}
            placeholder="Filter by name..."
          />
        </div>

        <div className="form-group">
          <label>Effort</label>
          <select value={filterEffort} onChange={e => setFilterEffort(parseInt(e.target.value))}>
            <option value={ALL_FILTER}>Any cost</option>
            {effortOptions.map(o => (
              <option key={o.id} value={o.id}>{o.displayName}</option>
            ))}
          </select>
        </div>

        {loading ? (
          <div>Loading arts...</div>
        ) : filteredArts.length === 0 ? (
          <div>No arts available to learn at your current level</div>
        ) : (
          <div style={{ marginTop: '1rem', display: 'grid', gap: '0.75rem' }}>
            {filteredArts.map(art => (
              <div key={art.id} style={{ border: '1px solid var(--border)', padding: '0.75rem', borderRadius: '0.25rem' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontWeight: 'bold' }}>{art.name}</div>
                    <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>
                      Min Level {art.minLevel} · Effort: {effortCommitmentById.get(art.effortCost)?.displayName ?? '—'}
                    </div>
                    {art.summary && (
                      <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                        {art.summary}
                      </div>
                    )}
                    <div style={{ fontSize: '0.875rem', marginTop: '0.25rem' }}>{art.description}</div>
                  </div>
                  <button className="sm" onClick={() => handleLearn(art.id)} style={{ marginLeft: '0.5rem' }}>Learn</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
