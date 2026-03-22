import { useState, useEffect } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import { focusDefinitionApi } from '../../api/focusDefinitionApi';
import { FocusDatabaseModal } from './FocusDatabaseModal';
import type { CharacterDetail, AddFocusRequest } from '../../types/character';
import type { FocusDefinition } from '../../types/focusDefinition';

export function FociPanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [showAdd, setShowAdd] = useState(false);
  const [showCatalog, setShowCatalog] = useState(false);
  const [name, setName] = useState('');
  const [level, setLevel] = useState(1);
  const [definitions, setDefinitions] = useState<FocusDefinition[]>([]);

  useEffect(() => {
    focusDefinitionApi.list().then(setDefinitions).catch(() => {});
  }, []);

  const getDefinition = (focusName: string) =>
    definitions.find(d => d.name.toLowerCase() === focusName.toLowerCase()) ?? null;

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

  const handleCatalogAdd = (updated: CharacterDetail) => {
    onUpdate(updated);
    setShowCatalog(false);
  };

  return (
    <SectionCard title="Foci">
      {character.foci.map(f => {
        const def = getDefinition(f.name);
        const description = def
          ? (f.level === 2 ? def.level2Description : def.level1Description)
          : null;

        return (
          <div key={f.id} className="focus-row">
            <div className="focus-header">
              <span><strong>{f.name}</strong> (Level {f.level})</span>
              <button className="sm danger" onClick={() => handleRemove(f.id)}>Remove</button>
            </div>
            {description && (
              <div className="focus-effects" style={{ fontSize: '0.875rem', color: 'var(--text-muted)', fontStyle: 'italic' }}>
                {description}
              </div>
            )}
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
        );
      })}

      {!showAdd && (
        <div style={{ display: 'flex', gap: '0.5rem', marginTop: '0.5rem' }}>
          <button className="sm" onClick={() => setShowCatalog(true)}>
            Browse Catalog
          </button>
          <button className="sm secondary" onClick={() => setShowAdd(true)}>
            + Add Manually
          </button>
        </div>
      )}

      {showAdd && (
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
      )}

      {showCatalog && (
        <FocusDatabaseModal
          character={character}
          onAdd={handleCatalogAdd}
          onClose={() => setShowCatalog(false)}
        />
      )}
    </SectionCard>
  );
}
