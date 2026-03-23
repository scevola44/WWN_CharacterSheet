import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';

const ATTRIBUTE_GROUPS: Record<string, string[]> = {
  Stab: ['Strength', 'Dexterity'],
  Shoot: ['Strength', 'Dexterity'],
  Punch: ['Strength', 'Dexterity'],
  Magic: ['Intelligence', 'Wisdom', 'Charisma'],
};

export function CombatStats({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate?: (c: CharacterDetail) => void;
}) {
  const { derivedStats } = character;
  const [updating, setUpdating] = useState<string | null>(null);

  const handleWeaponConfigChange = async (weaponId: string, skill: string, attribute: string) => {
    setUpdating(weaponId);
    try {
      const updated = await characterApi.updateWeaponAttackConfig(character.id, weaponId, skill, attribute);
      onUpdate?.(updated);
    } finally {
      setUpdating(null);
    }
  };

  const equippedWeapons = character.inventory.filter(
    i => i.itemType === 'Weapon' && i.slotType === 'Equipped'
  );

  return (
    <SectionCard title="Combat">
      <div className="combat-row">
        <div className="stat-box">
          <div className="label">AC</div>
          <div className="value">{derivedStats.armorClass}</div>
        </div>
        <div className="stat-box">
          <div className="label">BAB</div>
          <div className="value">+{derivedStats.baseAttackBonus}</div>
        </div>
        <div className="stat-box">
          <div className="label">Physical</div>
          <div className="value">{derivedStats.physicalSave}+</div>
        </div>
        <div className="stat-box">
          <div className="label">Evasion</div>
          <div className="value">{derivedStats.evasionSave}+</div>
        </div>
        <div className="stat-box">
          <div className="label">Mental</div>
          <div className="value">{derivedStats.mentalSave}+</div>
        </div>
      </div>

      {equippedWeapons.length > 0 && (
        <div style={{ marginTop: '0.75rem' }}>
          <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: '0.25rem' }}>
            EQUIPPED WEAPONS
          </div>
          {equippedWeapons.map(w => {
            const atkBonus = derivedStats.weaponAttackBonuses[w.id];
            const currentSkill = w.combatSkill || 'Stab';
            const currentAttr = w.attributeModifier || 'Strength';
            const validAttributes = ATTRIBUTE_GROUPS[currentSkill] || ['Strength', 'Dexterity'];

            return (
              <div key={w.id} className="item-row" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <div className="item-info">
                  <div className="item-name">{w.name}</div>
                  <div className="item-meta">
                    {w.damageDie} dmg | Atk {atkBonus !== undefined ? (atkBonus >= 0 ? `+${atkBonus}` : atkBonus) : '?'}
                    {w.shockDamage !== null && ` | Shock ${w.shockDamage}/${w.shockAcThreshold}`}
                  </div>
                </div>
                <div style={{ display: 'flex', gap: '0.5rem', fontSize: '0.75rem' }}>
                  <div style={{ flex: 1 }}>
                    <label style={{ display: 'block', marginBottom: '0.2rem', color: 'var(--text-muted)' }}>Skill</label>
                    <select
                      value={currentSkill}
                      onChange={(e) => handleWeaponConfigChange(w.id, e.target.value, currentAttr)}
                      disabled={updating === w.id}
                      style={{ width: '100%', padding: '0.25rem', fontSize: '0.75rem' }}
                    >
                      <option value="Stab">Stab</option>
                      <option value="Shoot">Shoot</option>
                      <option value="Punch">Punch</option>
                      <option value="Magic">Magic</option>
                    </select>
                  </div>
                  <div style={{ flex: 1 }}>
                    <label style={{ display: 'block', marginBottom: '0.2rem', color: 'var(--text-muted)' }}>Attribute</label>
                    <select
                      value={currentAttr}
                      onChange={(e) => handleWeaponConfigChange(w.id, currentSkill, e.target.value)}
                      disabled={updating === w.id}
                      style={{ width: '100%', padding: '0.25rem', fontSize: '0.75rem' }}
                    >
                      {validAttributes.map(attr => (
                        <option key={attr} value={attr}>{attr}</option>
                      ))}
                    </select>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </SectionCard>
  );
}
