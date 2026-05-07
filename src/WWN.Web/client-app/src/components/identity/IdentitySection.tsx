import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { LevelUpModal } from './LevelUpModal';
import type { CharacterDetail } from '../../types/character';

export function IdentitySection({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate?: (updated: CharacterDetail) => void;
}) {
  const [showLevelUp, setShowLevelUp] = useState(false);
  const classDisplay = character.class === 'Adventurer'
    ? `Adventurer (${character.partialClassA} / ${character.partialClassB})`
    : character.class;

  return (
    <SectionCard title="Identity">
      <div className="form-row">
        <div className="form-group">
          <label>Name</label>
          <strong>{character.name}</strong>
        </div>
        <div className="form-group">
          <label>Class</label>
          <strong>{classDisplay}</strong>
        </div>
        <div className="form-group">
          <label>Level</label>
          <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
            <strong>{character.level}</strong>
            {onUpdate && character.level < 10 && (
              <button className="sm secondary" onClick={() => setShowLevelUp(true)}>Level Up</button>
            )}
          </div>
        </div>
        <div className="form-group">
          <label>XP</label>
          <strong>{character.experiencePoints}</strong>
        </div>
      </div>
      {showLevelUp && onUpdate && (
        <LevelUpModal
          character={character}
          onClose={() => setShowLevelUp(false)}
          onUpdate={onUpdate}
        />
      )}
      {(character.background || character.origin) && (
        <div className="form-row">
          {character.background && (
            <div className="form-group">
              <label>Background</label>
              <span>{character.background}</span>
            </div>
          )}
          {character.origin && (
            <div className="form-group">
              <label>Origin</label>
              <span>{character.origin}</span>
            </div>
          )}
        </div>
      )}
    </SectionCard>
  );
}
