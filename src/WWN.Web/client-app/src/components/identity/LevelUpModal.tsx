import { useState } from 'react';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

export function LevelUpModal({ character, onClose, onUpdate }: {
  character: CharacterDetail;
  onClose: () => void;
  onUpdate: (updated: CharacterDetail) => void;
}) {
  const [hpRoll, setHpRoll] = useState(4);
  const [error, setError] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const conMod = character.derivedStats.attributeModifiers['Constitution'] ?? 0;
  const hitDieMod = character.derivedStats.hitDieModifier;
  const hpGain = Math.max(1, hpRoll + conMod + hitDieMod);

  const handleConfirm = async () => {
    setError('');
    setSubmitting(true);
    try {
      const updated = await characterApi.levelUp(character.id, hpGain);
      onUpdate(updated);
      onClose();
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Level up failed');
      setSubmitting(false);
    }
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }} onClick={onClose}>
      <div
        style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: 420, width: '100%', margin: '1rem' }}
        onClick={e => e.stopPropagation()}
      >
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>Level Up</h2>
          <button className="sm secondary" onClick={onClose}>✕</button>
        </div>

        <div className="form-group">
          <label>Level</label>
          <strong>{character.level} → {character.level + 1}</strong>
        </div>

        <div className="form-group">
          <label>HP roll (d6)</label>
          <input
            type="number"
            min={1}
            max={6}
            value={hpRoll}
            onChange={e => setHpRoll(Math.max(1, Math.min(6, +e.target.value)))}
            style={{ width: 80 }}
          />
        </div>

        <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginBottom: '1rem' }}>
          HP gain: <strong>+{hpGain}</strong> (roll {hpRoll}{conMod >= 0 ? ' + ' : ' − '}{Math.abs(conMod)} CON{hitDieMod >= 0 ? ' + ' : ' − '}{Math.abs(hitDieMod)} class, min 1)
        </div>

        {error && <div className="error" style={{ marginBottom: '0.75rem' }}>{error}</div>}

        <div style={{ display: 'flex', gap: '0.5rem' }}>
          <button className="secondary" onClick={onClose} disabled={submitting}>Cancel</button>
          <button onClick={handleConfirm} disabled={submitting}>Confirm</button>
        </div>
      </div>
    </div>
  );
}
