import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail, AddFocusRequest } from '../../types/character';

export function FociPanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [showAdd, setShowAdd] = useState(false);
  const [name, setName] = useState('');
  const [level, setLevel] = useState(1);

  const handleAdd = async () => {
    if (!name.trim()) return;
    const req: AddFocusRequest = { name: name.trim(), level, effects: [] };
    const updated = await characterApi.addFocus(character.id, req);
    onUpdate(updated);
    setShowAdd(false);
    setName('');
    setLevel(1);
  };

  const handleRemove = async (focusId: string) => {
    await characterApi.removeFocus(character.id, focusId);
    const updated = await characterApi.get(character.id);
    onUpdate(updated);
  };

  return (
    <SectionCard title="Foci">
      {character.foci.map(f => (
        <div key={f.id} className="focus-row">
          <div className="focus-header">
            <span><strong>{f.name}</strong> (Level {f.level})</span>
            <button className="sm danger" onClick={() => handleRemove(f.id)}>Remove</button>
          </div>
          {f.effects.length > 0 && (
            <div className="focus-effects">
              {f.effects.map((e, i) => (
                <div key={i}>
                  {e.type}: +{e.numericValue}
                  {e.targetSkill && ` (${e.targetSkill})`}
                  {e.targetAttribute && ` (${e.targetAttribute})`}
                  {e.description && ` - ${e.description}`}
                </div>
              ))}
            </div>
          )}
        </div>
      ))}

      {showAdd ? (
        <div style={{ marginTop: '0.5rem' }}>
          <div className="form-row">
            <div className="form-group">
              <label>Focus Name</label>
              <input value={name} onChange={e => setName(e.target.value)} placeholder="e.g. Alert" />
            </div>
            <div className="form-group">
              <label>Level</label>
              <select value={level} onChange={e => setLevel(+e.target.value)}>
                <option value={1}>1</option>
                <option value={2}>2</option>
              </select>
            </div>
          </div>
          <div className="modal-actions">
            <button className="secondary" onClick={() => setShowAdd(false)}>Cancel</button>
            <button onClick={handleAdd}>Add Focus</button>
          </div>
        </div>
      ) : (
        <button className="sm" style={{ marginTop: '0.5rem' }} onClick={() => setShowAdd(true)}>
          + Add Focus
        </button>
      )}
    </SectionCard>
  );
}
