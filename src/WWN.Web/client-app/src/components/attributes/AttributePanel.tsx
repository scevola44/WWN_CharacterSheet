import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

function formatMod(m: number) { return m >= 0 ? `+${m}` : `${m}`; }

export function AttributePanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
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

  return (
    <SectionCard title="Attributes">
      <div className="attr-grid">
        {character.attributes.map(attr => (
          <div key={attr.name} className="attr-box" onClick={() => {
            setEditing(attr.name);
            setEditVal(String(attr.score));
          }}>
            <div className="label">{attr.name.substring(0, 3)}</div>
            {editing === attr.name ? (
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
