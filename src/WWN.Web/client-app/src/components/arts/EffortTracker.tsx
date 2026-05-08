import type { CharacterDetail } from '../../types/character';
import { artsApi } from '../../api/artApi';
import { useLookups } from '../../contexts/LookupsContext';

export function EffortTracker({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const { effortCommitmentByCode } = useLookups();

  if (!character.effort) return null;

  const { max, scene, day, sustained } = character.effort;
  const used = scene + day + sustained;
  const free = max - used;

  const sceneId = effortCommitmentByCode.get('Scene')?.id;
  const dayId = effortCommitmentByCode.get('Day')?.id;
  const sustainedId = effortCommitmentByCode.get('Sustained')?.id;

  const handleCommit = async (id: number | undefined) => {
    if (id === undefined || free <= 0) return;
    const updated = await artsApi.commitEffort(character.id, id);
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
    commitId: number | undefined,
    inverse: { text: string; onClick: () => void; disabled: boolean } | null,
  ) => (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '0.5rem', fontSize: '0.9rem' }}>
      <span style={{ flex: 1 }}>{label}: {count}</span>
      <div style={{ display: 'flex', gap: '0.25rem' }}>
        <button
          className={`sm ${free <= 0 ? 'disabled' : ''}`}
          onClick={() => handleCommit(commitId)}
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
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.25rem' }}>
        <h3 style={{ margin: 0, fontSize: '1rem' }}>Effort: {free}/{max}</h3>
        <button className="sm" onClick={handleRest}>Rest</button>
      </div>
      <div style={{ display: 'grid', gap: '0.25rem' }}>
        {row('Scene', scene, sceneId, { text: 'End scene', onClick: handleEndScene, disabled: scene === 0 })}
        {row('Day', day, dayId, null)}
        {row('Sustained', sustained, sustainedId, { text: 'Release', onClick: handleRelease, disabled: sustained === 0 })}
      </div>
    </div>
  );
}
