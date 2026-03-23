import { useState, useEffect } from 'react';
import type { FocusDefinition } from '../../types/focusDefinition';
import type { CharacterDetail } from '../../types/character';
import { focusDefinitionApi } from '../../api/focusDefinitionApi';
import { characterApi } from '../../api/characterApi';

export function FocusDatabaseModal({ character, onAdd, onClose }: {
  character: CharacterDetail;
  onAdd: (updated: CharacterDetail) => void;
  onClose: () => void;
}) {
  const [foci, setFoci] = useState<FocusDefinition[]>([]);
  const [filtered, setFiltered] = useState<FocusDefinition[]>([]);
  const [searchName, setSearchName] = useState('');
  const [loading, setLoading] = useState(true);
  const [expandedId, setExpandedId] = useState<string | null>(null);
  const [levelSelections, setLevelSelections] = useState<Record<string, 1 | 2>>({});
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    focusDefinitionApi.list().then(setFoci).finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    const search = searchName.toLowerCase();
    setFiltered(
      foci.filter(fd => !search || fd.name.toLowerCase().includes(search))
    );
  }, [foci, searchName]);

  const getCharacterFocusLevel = (focusName: string): number =>
    Math.max(0, ...character.foci
      .filter(f => f.name.toLowerCase() === focusName.toLowerCase())
      .map(f => f.level));

  const isAvailable = (fd: FocusDefinition): boolean => {
    const currentLevel = getCharacterFocusLevel(fd.name);
    if (fd.canTakeMultipleTimes) return true;
    if (currentLevel === 0) return true;           // can take at level 1
    if (currentLevel === 1 && fd.hasLevel2) return true; // can upgrade to level 2
    return false;                                   // already at max
  };

  const getDefaultLevel = (fd: FocusDefinition): 1 | 2 => {
    const currentLevel = getCharacterFocusLevel(fd.name);
    if (currentLevel >= 1 && fd.hasLevel2) return 2;
    return 1;
  };

  const getSelectedLevel = (fd: FocusDefinition): 1 | 2 =>
    levelSelections[fd.id] ?? getDefaultLevel(fd);

  const handleAdd = async (fd: FocusDefinition) => {
    setError(null);
    try {
      const level = getSelectedLevel(fd);
      const existingFocus = character.foci.find(
        f => f.name.toLowerCase() === fd.name.toLowerCase()
      );

      if (existingFocus && level === 2 && existingFocus.level === 1) {
        // Upgrade existing L1 focus to L2 in-place
        const updated = await characterApi.upgradeFocus(
          character.id,
          existingFocus.id,
          fd.level2Effects
        );
        onAdd(updated);
      } else {
        // Add fresh focus with effects for the selected level
        const effects = level === 2
          ? [...fd.level1Effects, ...fd.level2Effects]
          : fd.level1Effects;
        const updated = await characterApi.addFocus(character.id, {
          name: fd.name,
          level,
          effects,
        });
        onAdd(updated);
      }
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to add focus');
    }
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: '640px', width: '100%', maxHeight: '85vh', overflow: 'auto' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>Select a Focus</h2>
          <button className="sm danger" onClick={onClose}>✕</button>
        </div>

        {error && (
          <div style={{ color: 'var(--danger)', marginBottom: '0.5rem', fontSize: '0.875rem' }}>{error}</div>
        )}

        <div className="form-group">
          <label>Search</label>
          <input
            type="text"
            value={searchName}
            onChange={e => setSearchName(e.target.value)}
            placeholder="Filter by name..."
            autoFocus
          />
        </div>

        {loading ? (
          <div>Loading foci...</div>
        ) : filtered.length === 0 ? (
          <div style={{ color: 'var(--text-muted)', padding: '1rem 0' }}>No foci found.</div>
        ) : (
          <div style={{ display: 'grid', gap: '0.75rem', marginTop: '1rem' }}>
            {filtered.map(fd => {
              const currentLevel = getCharacterFocusLevel(fd.name);
              const available = isAvailable(fd);
              const selectedLevel = getSelectedLevel(fd);
              const isExpanded = expandedId === fd.id;

              return (
                <div
                  key={fd.id}
                  style={{
                    border: '1px solid var(--border)',
                    borderRadius: '0.25rem',
                    overflow: 'hidden',
                    opacity: available ? 1 : 0.5,
                  }}
                >
                  {/* Header row */}
                  <div
                    style={{ padding: '0.75rem', cursor: 'pointer', background: 'var(--bg)' }}
                    onClick={() => setExpandedId(isExpanded ? null : fd.id)}
                  >
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                      <div>
                        <div style={{ fontWeight: 'bold' }}>{fd.name}</div>
                        <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.125rem', display: 'flex', gap: '0.75rem' }}>
                          {fd.hasLevel2 ? <span>Levels 1–2</span> : <span>Level 1 only</span>}
                          {fd.canTakeMultipleTimes && <span style={{ color: 'var(--accent)' }}>Can take multiple times</span>}
                          {currentLevel > 0 && (
                            <span style={{ color: 'var(--text-muted)' }}>
                              Currently: Level {currentLevel}
                              {currentLevel >= 2 || (!fd.hasLevel2 && !fd.canTakeMultipleTimes) ? ' (max)' : ''}
                            </span>
                          )}
                        </div>
                        {fd.description && (
                          <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', fontStyle: 'italic', marginTop: '0.25rem' }}>
                            {fd.description}
                          </div>
                        )}
                      </div>
                      <span style={{ color: 'var(--text-muted)', fontSize: '0.75rem' }}>{isExpanded ? '▲' : '▼'}</span>
                    </div>
                  </div>

                  {/* Expanded details + add controls */}
                  {isExpanded && (
                    <div style={{ padding: '0.75rem', borderTop: '1px solid var(--border)' }}>
                      <div style={{ marginBottom: '0.75rem', display: 'grid', gap: '0.5rem' }}>
                        <div>
                          <div style={{ fontSize: '0.75rem', fontWeight: 'bold', textTransform: 'uppercase', color: 'var(--text-muted)', marginBottom: '0.25rem' }}>
                            Level 1
                          </div>
                          <p style={{ margin: 0, fontSize: '0.875rem' }}>{fd.level1Description}</p>
                        </div>
                        {fd.hasLevel2 && fd.level2Description && (
                          <div>
                            <div style={{ fontSize: '0.75rem', fontWeight: 'bold', textTransform: 'uppercase', color: 'var(--text-muted)', marginBottom: '0.25rem' }}>
                              Level 2
                            </div>
                            <p style={{ margin: 0, fontSize: '0.875rem' }}>{fd.level2Description}</p>
                          </div>
                        )}
                      </div>

                      {available && (
                        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem', borderTop: '1px solid var(--border)', paddingTop: '0.75rem' }}>
                          {(fd.hasLevel2 || fd.canTakeMultipleTimes) && (
                            <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                              <label style={{ fontSize: '0.875rem', marginBottom: 0 }}>Level:</label>
                              <select
                                value={selectedLevel}
                                onChange={e =>
                                  setLevelSelections(prev => ({
                                    ...prev,
                                    [fd.id]: parseInt(e.target.value) as 1 | 2,
                                  }))
                                }
                              >
                                <option value={1}>1</option>
                                {fd.hasLevel2 && <option value={2}>2</option>}
                              </select>
                            </div>
                          )}
                          <button className="sm" onClick={() => handleAdd(fd)}>
                            Add to Character
                          </button>
                        </div>
                      )}
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
