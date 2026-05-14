import { useEffect, useState } from 'react';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail, ItemInfo } from '../../types/character';

interface ItemNoteModalProps {
  item: ItemInfo;
  characterId: string;
  onClose: () => void;
  onSaved: (updated: CharacterDetail) => void;
}

export function ItemNoteModal({ item, characterId, onClose, onSaved }: ItemNoteModalProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [draft, setDraft] = useState(item.description ?? '');

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => { if (e.key === 'Escape') onClose(); };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [onClose]);

  const handleSave = async () => {
    const updated = await characterApi.updateItem(characterId, item.id, {
      name: item.name,
      description: draft || undefined,
      encumbrance: item.encumbrance,
      quantity: item.quantity,
      itemType: item.itemType,
      damageDieCount: item.itemType === 'Weapon' ? parseInt(item.damageDie?.split('d')[0] || '1') : undefined,
      damageDieSides: item.itemType === 'Weapon' ? parseInt(item.damageDie?.split('d')[1] || '6') : undefined,
      attributeModifier: item.attributeModifier ?? undefined,
      combatSkill: item.combatSkill ?? undefined,
      shockDamage: item.shockDamage ?? undefined,
      shockAcThreshold: item.shockAcThreshold ?? undefined,
      tags: item.tags ?? undefined,
      acBonus: item.acBonus ?? undefined,
      isShield: item.isShield ?? undefined,
    });
    onSaved(updated);
    setIsEditing(false);
  };

  const handleCancelEdit = () => {
    setDraft(item.description ?? '');
    setIsEditing(false);
  };

  return (
    <div className="modal-overlay" style={{ zIndex: 1000 }} onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()} style={{ minWidth: '320px', maxWidth: '480px' }}>
        <h3 style={{ marginBottom: '0.75rem' }}>{item.name}</h3>

        {isEditing ? (
          <>
            <textarea
              autoFocus
              rows={6}
              style={{ width: '100%', resize: 'vertical' }}
              value={draft}
              onChange={e => setDraft(e.target.value)}
              placeholder="Write a note…"
            />
            <div className="modal-actions">
              <button className="secondary" onClick={handleCancelEdit}>Cancel</button>
              <button onClick={handleSave}>Save</button>
            </div>
          </>
        ) : (
          <>
            <p style={{ whiteSpace: 'pre-wrap', margin: '0 0 1rem', minHeight: '3rem' }}>
              {item.description || <em style={{ color: 'var(--text-muted)' }}>No note.</em>}
            </p>
            <div className="modal-actions">
              <button className="secondary" onClick={onClose}>Close</button>
              <button onClick={() => setIsEditing(true)}>Edit</button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
