import { SectionCard } from '../layout/SectionCard';
import type { CharacterDetail } from '../../types/character';

export function IdentitySection({ character }: { character: CharacterDetail }) {
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
          <strong>{character.level}</strong>
        </div>
        <div className="form-group">
          <label>XP</label>
          <strong>{character.experiencePoints}</strong>
        </div>
      </div>
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
