import { useState, useEffect } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

function formatMod(m: number) { return m >= 0 ? `+${m}` : `${m}`; }

export function AttributePanel({ character, onUpdate, isEditing }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
  isEditing: boolean;
}) {
  const [editing, setEditing] = useState<string | null>(null);
  const [editVal, setEditVal] = useState('');

  const handleSave = async (name: string) => {
    const score = parseInt(editVal);
    if (score >= 3 && score <= 18) {
      const updated = await characterApi.updateAttribute(character.id, name, score);
      onUpdate(updated);
    }
    setEditing(null);
  };

  useEffect(() => {
    if (!isEditing) setEditing(null);
  }, [isEditing]);

  const handleClick = (attr: { name: string; score: number }) => {
    if (!isEditing) return;
    setEditing(attr.name);
    setEditVal(String(attr.score));
  };

  return (
    <SectionCard title="Attributes">
      <div className="attr-grid">
        {character.attributes.map(attr => (
          <div
            key={attr.name}
            className="attr-box"
            onClick={() => handleClick(attr)}
            style={isEditing ? undefined : { cursor: 'default' }}
          >
            <div className="label">{attr.name.substring(0, 3)}</div>
            {isEditing && editing === attr.name ? (
              <input
                type="number"
                value={editVal}
                onChange={e => setEditVal(e.target.value)}
                onBlur={() => handleSave(attr.name)}
                onKeyDown={e => e.key === 'Enter' && handleSave(attr.name)}
                autoFocus
                min={3}
                max={18}
              />
            ) : (
              <>
                <div className="score">{attr.score}</div>
                <div className="mod">{formatMod(attr.modifier)}</div>
              </>
            )}
          </div>
        ))}
      </div>
    </SectionCard>
  );
}
