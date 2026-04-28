import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

export function StrainTracker({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [editValue, setEditValue] = useState(character.currentStrain);

  const maxStrain = character.derivedStats.maxStrain;
  const pct = maxStrain > 0
    ? Math.round((character.currentStrain / maxStrain) * 100)
    : 0;

  const handleDelta = async (delta: number) => {
    const newVal = Math.max(0, Math.min(maxStrain, character.currentStrain + delta));
    const updated = await characterApi.setStrain(character.id, newVal);
    onUpdate(updated);
  };

  const handleSave = async () => {
    const clamped = Math.max(0, Math.min(maxStrain, editValue));
    const updated = await characterApi.setStrain(character.id, clamped);
    onUpdate(updated);
    setEditing(false);
  };

  return (
    <SectionCard title="System Strain">
      <div className="hp-tracker">
        <button className="sm secondary" onClick={() => handleDelta(-1)}>-1</button>
        <div className="hp-display" onClick={() => {
          setEditValue(character.currentStrain);
          setEditing(true);
        }}>
          {editing ? (
            <span>
              <input
                type="number"
                value={editValue}
                onChange={e => setEditValue(+e.target.value)}
                style={{ width: 50 }}
              />
              {' / '}
              {maxStrain}
              <button className="sm" onClick={handleSave} style={{ marginLeft: 4 }}>Save</button>
            </span>
          ) : (
            `${character.currentStrain} / ${maxStrain}`
          )}
        </div>
        <div className="hp-bar">
          <div className="hp-fill" style={{ width: `${pct}%` }} />
        </div>
        <button className="sm secondary" onClick={() => handleDelta(1)}>+1</button>
      </div>
      <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.5rem' }}>
        Max = CON score ({maxStrain}) | Recovers 1 per night's rest
      </div>
    </SectionCard>
  );
}
