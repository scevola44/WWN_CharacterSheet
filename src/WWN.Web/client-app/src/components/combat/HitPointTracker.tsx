import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

export function HitPointTracker({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [maxHp, setMaxHp] = useState(character.maxHitPoints);
  const [curHp, setCurHp] = useState(character.currentHitPoints);

  const pct = character.maxHitPoints > 0
    ? Math.round((character.currentHitPoints / character.maxHitPoints) * 100)
    : 0;

  const handleDelta = async (delta: number) => {
    const newCur = Math.max(0, Math.min(character.maxHitPoints, character.currentHitPoints + delta));
    const updated = await characterApi.setHp(character.id, character.maxHitPoints, newCur);
    onUpdate(updated);
  };

  const handleSave = async () => {
    const updated = await characterApi.setHp(character.id, maxHp, Math.min(curHp, maxHp));
    onUpdate(updated);
    setEditing(false);
  };

  return (
    <SectionCard title="Hit Points">
      <div className="hp-tracker">
        <button className="sm secondary" onClick={() => handleDelta(-1)}>-1</button>
        <button className="sm secondary" onClick={() => handleDelta(-5)}>-5</button>
        <div className="hp-display" onClick={() => {
          setMaxHp(character.maxHitPoints);
          setCurHp(character.currentHitPoints);
          setEditing(true);
        }}>
          {editing ? (
            <span>
              <input type="number" value={curHp} onChange={e => setCurHp(+e.target.value)} style={{ width: 50 }} />
              {' / '}
              <input type="number" value={maxHp} onChange={e => setMaxHp(+e.target.value)} style={{ width: 50 }} />
              <button className="sm" onClick={handleSave} style={{ marginLeft: 4 }}>Save</button>
            </span>
          ) : (
            `${character.currentHitPoints} / ${character.maxHitPoints}`
          )}
        </div>
        <div className="hp-bar">
          <div className="hp-fill" style={{ width: `${pct}%` }} />
        </div>
        <button className="sm secondary" onClick={() => handleDelta(5)}>+5</button>
        <button className="sm secondary" onClick={() => handleDelta(1)}>+1</button>
      </div>
      <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginTop: '0.5rem' }}>
        Hit Die: d6{character.derivedStats.hitDieModifier >= 0 ? '+' : ''}{character.derivedStats.hitDieModifier}
      </div>
    </SectionCard>
  );
}
