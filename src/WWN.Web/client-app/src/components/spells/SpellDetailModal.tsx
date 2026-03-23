import { useState } from 'react';
import type { Spell, UpdateSpellRequest } from '../../types/spell';
import { spellsApi } from '../../api/spellApi';
import { SpellForm } from './SpellForm';

export function SpellDetailModal({ spell, onClose, onSaved }: {
  spell: Spell;
  onClose: () => void;
  onSaved: (updated: Spell) => void;
}) {
  const [editing, setEditing] = useState(false);
  const [editForm, setEditForm] = useState<UpdateSpellRequest>({
    name: spell.name,
    spellLevel: spell.spellLevel,
    description: spell.description,
    summary: spell.summary ?? '',
  });

  const handleSave = async () => {
    const updated = await spellsApi.update(spell.id, editForm);
    onSaved(updated);
    setEditing(false);
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000 }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: 600, width: '100%', maxHeight: '80vh', overflowY: 'auto', margin: '1rem' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2 style={{ margin: 0 }}>{spell.name}</h2>
          <button className="sm secondary" onClick={onClose}>✕</button>
        </div>

        {editing ? (
          <SpellForm
            values={editForm}
            onChange={setEditForm}
            onSubmit={handleSave}
            onCancel={() => setEditing(false)}
            submitLabel="Save"
          />
        ) : (
          <>
            <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginBottom: '0.75rem' }}>Level {spell.spellLevel}</div>

            {spell.summary && (
              <p style={{ margin: '0 0 0.75rem 0', fontStyle: 'italic', color: 'var(--text-muted)' }}>{spell.summary}</p>
            )}

            <p style={{ margin: '0 0 1.25rem 0', fontSize: '0.9rem' }}>{spell.description}</p>

            <button className="secondary" onClick={() => setEditing(true)}>Edit</button>
          </>
        )}
      </div>
    </div>
  );
}
