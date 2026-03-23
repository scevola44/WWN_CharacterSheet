import { useState } from 'react';
import type { FocusDefinition, UpdateFocusDefinitionRequest } from '../../types/focusDefinition';
import { focusDefinitionApi } from '../../api/focusDefinitionApi';
import { FocusForm } from './FocusForm';

export function FocusDetailModal({ focus, onClose, onSaved }: {
  focus: FocusDefinition;
  onClose: () => void;
  onSaved: (updated: FocusDefinition) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [editForm, setEditForm] = useState<UpdateFocusDefinitionRequest>({
    name: focus.name,
    description: focus.description ?? '',
    level1Description: focus.level1Description,
    level2Description: focus.level2Description ?? '',
    canTakeMultipleTimes: focus.canTakeMultipleTimes,
    level1Effects: focus.level1Effects,
    level2Effects: focus.level2Effects,
  });

  const handleSave = async () => {
    const updated = await focusDefinitionApi.update(focus.id, editForm);
    onSaved(updated);
    setEditing(false);
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: 640, width: '100%', maxHeight: '85vh', overflowY: 'auto', margin: '1rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>{focus.name}</h2>
          <button className="sm secondary" onClick={onClose}>✕</button>
        </div>

        {editing ? (
          <>
            <FocusForm
              values={editForm}
              onChange={setEditForm}
              onSubmit={handleSave}
              onCancel={() => setEditing(false)}
              submitLabel="Save"
            />
          </>
        ) : (
          <>
            <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginBottom: '1rem', display: 'flex', gap: '0.75rem' }}>
              {focus.hasLevel2 ? <span>Levels 1–2</span> : <span>Level 1 only</span>}
              {focus.canTakeMultipleTimes && <span style={{ color: 'var(--accent)' }}>Can take multiple times</span>}
            </div>

            {focus.description && (
              <p style={{ margin: '0 0 1rem 0', fontStyle: 'italic', color: 'var(--text-muted)' }}>{focus.description}</p>
            )}

            <div style={{ marginBottom: '0.75rem' }}>
              <div style={{ fontWeight: '600', marginBottom: '0.25rem' }}>Level 1</div>
              <p style={{ margin: 0, fontSize: '0.9rem' }}>{focus.level1Description}</p>
            </div>

            {focus.level2Description && (
              <div style={{ marginBottom: '0.75rem' }}>
                <div style={{ fontWeight: '600', marginBottom: '0.25rem' }}>Level 2</div>
                <p style={{ margin: 0, fontSize: '0.9rem' }}>{focus.level2Description}</p>
              </div>
            )}

            <div style={{ marginTop: '1.25rem' }}>
              <button className="secondary" onClick={() => setEditing(true)}>Edit</button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
