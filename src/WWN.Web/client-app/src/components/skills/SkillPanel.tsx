import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

export function SkillPanel({ character, onUpdate, isEditing }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
  isEditing: boolean;
}) {
  const handleRankChange = async (skillName: string, delta: number) => {
    const skill = character.skills.find(s =>
      s.name === skillName || (s.name === 'Custom' && s.customName === skillName));
    if (!skill) return;
    const newRank = skill.level + delta;
    if (newRank < -1 || newRank > 4) return;
    const updated = await characterApi.updateSkill(character.id, skill.name, newRank);
    onUpdate(updated);
  };

  return (
    <SectionCard title="Skills">
      <div className="skill-grid">
        {character.skills.map(skill => {
          const display = skill.name === 'Custom' ? skill.customName || 'Custom' : skill.name;
          return (
            <div key={skill.id} className="skill-row">
              <span className="name">{display}</span>
              <span>
                {isEditing && <button className="sm secondary" onClick={() => handleRankChange(skill.name, -1)}>-</button>}
                <span className="rank">{skill.level}</span>
                {isEditing && <button className="sm secondary" onClick={() => handleRankChange(skill.name, 1)}>+</button>}
              </span>
            </div>
          );
        })}
      </div>
    </SectionCard>
  );
}
