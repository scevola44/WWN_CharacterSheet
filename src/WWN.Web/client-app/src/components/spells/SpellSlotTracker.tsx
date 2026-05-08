import type { CharacterDetail } from '../../types/character';
import { spellsApi } from '../../api/spellApi';

export function SpellSlotTracker({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  if (!character.spellSlots) return null;

  const handleUseSlot = async (level: number) => {
    if (character.spellSlots!.used[level - 1] < character.spellSlots!.available[level - 1]) {
      const updated = await spellsApi.useSpellSlot(character.id, level);
      onUpdate(updated);
    }
  };

  const handleRestore = async () => {
    const updated = await spellsApi.restoreSpellSlots(character.id);
    onUpdate(updated);
  };

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '0.25rem' }}>
        <h3 style={{ margin: 0, fontSize: '1rem' }}>Spell Slots</h3>
        <button className="sm" onClick={handleRestore}>Rest</button>
      </div>
      <div style={{ display: 'grid', gap: '0.25rem' }}>
        {[1, 2, 3, 4, 5, 6].map(level => {
          const available = character.spellSlots!.available[level - 1];
          const used = character.spellSlots!.used[level - 1];
          const canUse = used < available;
          return (
            <div key={level} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '0.9rem' }}>
              <span>Level {level}: {available - used}/{available}</span>
              <button
                className={`sm ${!canUse ? 'disabled' : ''}`}
                onClick={() => handleUseSlot(level)}
                disabled={!canUse}
              >
                Use
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
}
