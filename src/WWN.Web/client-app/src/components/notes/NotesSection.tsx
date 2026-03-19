import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

export function NotesSection({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [notes, setNotes] = useState(character.notes ?? '');
  const [dirty, setDirty] = useState(false);

  const handleSave = async () => {
    const updated = await characterApi.updateNotes(character.id, notes || null);
    onUpdate(updated);
    setDirty(false);
  };

  return (
    <SectionCard title="Notes">
      <textarea
        className="notes-area"
        value={notes}
        onChange={e => { setNotes(e.target.value); setDirty(true); }}
        placeholder="Character notes..."
      />
      {dirty && (
        <button className="sm" style={{ marginTop: '0.5rem' }} onClick={handleSave}>
          Save Notes
        </button>
      )}
    </SectionCard>
  );
}
