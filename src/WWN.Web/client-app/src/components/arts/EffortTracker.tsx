import type { CharacterDetail } from '../../types/character';
import type { EffortCommitment } from '../../types/art';
import { artsApi } from '../../api/artApi';

export function EffortTracker({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  if (!character.effort) return null;

  const { max, scene, day, sustained } = character.effort;
  const used = scene + day + sustained;
  const free = max - used;

  const handleCommit = async (kind: EffortCommitment) => {
    if (free <= 0) return;
    const updated = await artsApi.commitEffort(character.id, kind);
    onUpdate(updated);
  };

  const handleEndScene = async () => {
    const updated = await artsApi.endScene(character.id);
    onUpdate(updated);
  };

  const handleRest = async () => {
    const updated = await artsApi.restDay(character.id);
    onUpdate(updated);
  };

  const handleRelease = async () => {
    if (sustained <= 0) return;
    const updated = await artsApi.releaseSustained(character.id);
    onUpdate(updated);
  };

  const row = (
    label: string,
    count: number,
    commit: EffortCommitment,
    inverse: { text: string; onClick: () => void; disabled: boolean } | null,
  ) => (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '0.5rem' }}>
      <span style={{ flex: 1 }}>{label}: {count}</span>
      <div style={{ display: 'flex', gap: '0.25rem' }}>
        <button
          className={`sm ${free <= 0 ? 'disabled' : ''}`}
          onClick={() => handleCommit(commit)}
          disabled={free <= 0}
        >
          Commit
        </button>
        {inverse && (
          <button
            className={`sm secondary ${inverse.disabled ? 'disabled' : ''}`}
            onClick={inverse.onClick}
            disabled={inverse.disabled}
          >
            {inverse.text}
          </button>
        )}
      </div>
    </div>
  );

  return (
    <div style={{ marginTop: '1rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.5rem' }}>
        <h3>Effort: {free}/{max}</h3>
        <button className="sm" onClick={handleRest}>Rest</button>
      </div>
      <div style={{ display: 'grid', gap: '0.5rem' }}>
        {row('Scene', scene, 'Scene', { text: 'End scene', onClick: handleEndScene, disabled: scene === 0 })}
        {row('Day', day, 'Day', null)}
        {row('Sustained', sustained, 'Sustained', { text: 'Release', onClick: handleRelease, disabled: sustained === 0 })}
      </div>
    </div>
  );
}
