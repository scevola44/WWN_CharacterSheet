import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { SpellSlotTracker } from './SpellSlotTracker';
import { SpellDatabaseModal } from './SpellDatabaseModal';
import { SpellDetailModal } from './SpellDetailModal';
import type { CharacterDetail } from '../../types/character';
import type { Spell } from '../../types/spell';
import { spellsApi } from '../../api/spellApi';

export function SpellsPanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedSpell, setSelectedSpell] = useState<Spell | null>(null);

  // Only show for Mage or Adventurer with PartialMage
  const isMage = character.class === 'Mage';
  const isPartialMage = character.partialClassA === 'PartialMage' || character.partialClassB === 'PartialMage';

  if (!isMage && !isPartialMage) return null;

  const handleForgetSpell = async (spellId: string) => {
    await spellsApi.forgetSpell(character.id, spellId);
    const updated = await spellsApi.getCharacterSpells(character.id);
    onUpdate(updated);
  };

  const handleLearnSuccess = async () => {
    setShowAddModal(false);
    const updated = await spellsApi.getCharacterSpells(character.id);
    onUpdate(updated);
  };

  return (
    <SectionCard title="Spells">
      <SpellSlotTracker character={character} onUpdate={onUpdate} />

      <div style={{ marginTop: '0.75rem' }}>
        <h3 style={{ margin: '0 0 0.25rem 0', fontSize: '0.9rem' }}>Spellbook</h3>
        {character.spellbook.length === 0 ? (
          <div style={{ color: 'var(--text-muted)', fontStyle: 'italic', fontSize: '0.9rem' }}>No spells learned</div>
        ) : (
          <div style={{ display: 'grid', gap: '0.25rem', marginTop: '0.25rem' }}>
            {character.spellbook.map(known => (
              <div key={known.id} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '0.25rem 0.375rem', background: 'var(--bg-subtle)', borderRadius: '0.25rem', fontSize: '0.85rem' }}>
                <div style={{ flex: 1 }}>
                  <button
                    style={{ background: 'none', border: 'none', padding: 0, cursor: 'pointer', fontWeight: '500', textDecoration: 'underline', color: 'inherit', fontSize: 'inherit' }}
                    onClick={() => setSelectedSpell(known.spell)}
                  >
                    {known.spell.name}
                  </button>
                  {known.spell.summary && <span style={{ fontSize: '0.7rem', marginLeft: '0.5rem', color: 'var(--primary)' }}>({known.spell.summary})</span>}
                  <span style={{ fontSize: '0.7rem', marginLeft: '0.5rem', color: 'var(--text-muted)' }}>L{known.spell.spellLevel}</span>
                </div>
                <button
                  className="sm danger"
                  onClick={() => handleForgetSpell(known.spellId)}
                >
                  Forget
                </button>
              </div>
            ))}
          </div>
        )}
      </div>

      <div style={{ marginTop: '0.5rem' }}>
        <button onClick={() => setShowAddModal(true)}>+ Add Spell</button>
      </div>

      {showAddModal && (
        <SpellDatabaseModal
          character={character}
          onLearn={handleLearnSuccess}
          onClose={() => setShowAddModal(false)}
        />
      )}

      {selectedSpell && (
        <SpellDetailModal
          spell={selectedSpell}
          onClose={() => setSelectedSpell(null)}
          onSaved={async updated => {
            setSelectedSpell(updated);
            const refreshed = await spellsApi.getCharacterSpells(character.id);
            onUpdate(refreshed);
          }}
        />
      )}
    </SectionCard>
  );
}
