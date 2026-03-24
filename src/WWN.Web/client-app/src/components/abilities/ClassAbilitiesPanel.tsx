import { SectionCard } from '../layout/SectionCard';
import type { CharacterDetail } from '../../types/character';

export function ClassAbilitiesPanel({ character }: { character: CharacterDetail }) {
  if (character.classAbilities.length === 0) return null;

  return (
    <SectionCard title="Class Abilities">
      {character.classAbilities.map((ability, i) => (
        <div key={i} className="focus-row">
          <div className="focus-header">
            <strong>
              {ability.name}
              {ability.minLevel > 1 && (
                <span
                  style={{
                    marginLeft: '0.5rem',
                    fontSize: '0.75rem',
                    fontWeight: 'normal',
                    color: 'var(--text-muted)',
                  }}
                >
                  (Lvl {ability.minLevel}+)
                </span>
              )}
            </strong>
          </div>
          <div
            className="focus-effects"
            style={{ fontSize: '0.875rem', color: 'var(--text-muted)', fontStyle: 'italic' }}
          >
            {ability.description}
          </div>
        </div>
      ))}
    </SectionCard>
  );
}
