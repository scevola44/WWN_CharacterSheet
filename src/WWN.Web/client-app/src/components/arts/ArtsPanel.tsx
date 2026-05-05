import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { EffortTracker } from './EffortTracker';
import { ArtDatabaseModal } from './ArtDatabaseModal';
import { ArtDetailModal } from './ArtDetailModal';
import type { CharacterDetail } from '../../types/character';
import type { Art } from '../../types/art';
import { artsApi } from '../../api/artApi';
import { characterApi } from '../../api/characterApi';

export function ArtsPanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedArt, setSelectedArt] = useState<Art | null>(null);

  const isMage = character.class === 'Mage';
  const isPartialMage =
    character.partialClassA === 'PartialMage' || character.partialClassB === 'PartialMage';

  if (!isMage && !isPartialMage) return null;

  const refresh = async () => {
    const updated = await characterApi.get(character.id);
    onUpdate(updated);
  };

  const handleForgetArt = async (artId: string) => {
    await artsApi.forgetArt(character.id, artId);
    await refresh();
  };

  const handleLearnSuccess = async () => {
    setShowAddModal(false);
    await refresh();
  };

  return (
    <SectionCard title="Arts">
      <EffortTracker character={character} onUpdate={onUpdate} />

      <div style={{ marginTop: '1.5rem' }}>
        <h3>Known Arts</h3>
        {character.arts.length === 0 ? (
          <div style={{ color: 'var(--text-muted)', fontStyle: 'italic' }}>No arts learned</div>
        ) : (
          <div style={{ display: 'grid', gap: '0.5rem', marginTop: '0.5rem' }}>
            {character.arts.map(known => (
              <div key={known.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '0.5rem', background: 'var(--bg-subtle)', borderRadius: '0.25rem' }}>
                <div style={{ flex: 1 }}>
                  <div style={{ fontWeight: '500' }}>
                    <button
                      style={{ background: 'none', border: 'none', padding: 0, cursor: 'pointer', fontWeight: '500', textDecoration: 'underline', color: 'inherit', fontSize: 'inherit' }}
                      onClick={() => setSelectedArt(known.art)}
                    >
                      {known.art.name}
                    </button>
                    {known.art.summary && <span style={{ fontSize: '0.75rem', marginLeft: '0.5rem', color: 'var(--primary)' }}>{known.art.summary}</span>}
                  </div>
                  <div style={{ fontSize: '0.75rem', color: 'var(--text-muted)' }}>
                    Min Level {known.art.minLevel} · {known.art.effortCost ? `Effort: ${known.art.effortCost}` : 'No effort'}
                  </div>
                </div>
                <button
                  className="sm danger"
                  onClick={() => handleForgetArt(known.artId)}
                >
                  Forget
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      <div style={{ marginTop: '1rem' }}>
        <button onClick={() => setShowAddModal(true)}>+ Add Art</button>
      </div>

      {showAddModal && (
        <ArtDatabaseModal
          character={character}
          onLearn={handleLearnSuccess}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {selectedArt && (
        <ArtDetailModal
          art={selectedArt}
          onClose={() => setSelectedArt(null)}
          onSaved={async updated => {
            setSelectedArt(updated);
            await refresh();
          }}
        />
      )}
    </SectionCard>
  );
}
