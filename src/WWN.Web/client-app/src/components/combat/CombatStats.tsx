import { useEffect, useMemo, useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail } from '../../types/character';
import { useWeaponTags } from '../../contexts/LookupsContext';
import type { LookupValue } from '../../types/lookups';

const ALL_ATTRIBUTES = ['Strength', 'Dexterity', 'Intelligence', 'Wisdom', 'Charisma', 'Constitution'];

function WeaponTagBadge({ tag }: { tag: LookupValue }) {
  const [open, setOpen] = useState(false);
  const hasTooltip = !!tag.description;

  useEffect(() => {
    if (!open) return;
    const close = () => setOpen(false);
    document.addEventListener('click', close);
    return () => document.removeEventListener('click', close);
  }, [open]);

  return (
    <span
      className={`weapon-tag-badge${hasTooltip ? ' weapon-tag-badge--interactive' : ''}`}
      onMouseEnter={hasTooltip ? () => setOpen(true) : undefined}
      onMouseLeave={hasTooltip ? () => setOpen(false) : undefined}
      onClick={hasTooltip ? e => {
        // Only handle tap on touch devices; mouse hover already handles desktop
        if ((e.nativeEvent as PointerEvent).pointerType !== 'mouse') {
          e.stopPropagation();
          setOpen(o => !o);
        }
      } : undefined}
    >
      {tag.abbreviation ?? tag.code}
      {open && tag.description && (
        <span className="weapon-tag-tooltip">
          <strong>{tag.displayName}</strong>
          {tag.description}
        </span>
      )}
    </span>
  );
}

export function CombatStats({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate?: (c: CharacterDetail) => void;
}) {
  const { derivedStats } = character;
  const [updating, setUpdating] = useState<string | null>(null);
  const allWeaponTags = useWeaponTags();
  const tagByCode = useMemo(
    () => new Map(allWeaponTags.map(t => [t.code, t])),
    [allWeaponTags]
  );

  const handleWeaponConfigChange = async (weaponId: string, skill: string, attribute: string) => {
    setUpdating(weaponId);
    try {
      const updated = await characterApi.updateWeaponAttackConfig(character.id, weaponId, skill, attribute);
      onUpdate?.(updated);
    } finally {
      setUpdating(null);
    }
  };

  const handleConditionalToggle = async (focusId: string, currentActive: boolean) => {
    const updated = await characterApi.setFocusConditional(character.id, focusId, !currentActive);
    onUpdate?.(updated);
  };

  const equippedWeapons = character.inventory.filter(
    i => i.itemType === 'Weapon' && i.slotType === 'Equipped'
  );

  const conditionalFoci = character.foci.filter(
    f => f.effects.some(e => e.condition === 'Conditional')
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
        <div className="stat-box">
          <div className="label">Luck</div>
          <div className="value">{derivedStats.luckSave}+</div>
        </div>
      </div>

      {equippedWeapons.length > 0 && (
        <div style={{ marginTop: '0.75rem' }}>
          <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: '0.25rem' }}>
            EQUIPPED WEAPONS
          </div>
          {equippedWeapons.map(w => {
            const atkBonus = derivedStats.weaponAttackBonuses[w.id];
            const dmgBonus = derivedStats.weaponDamageBonuses?.[w.id] ?? 0;
            const currentSkill = w.combatSkill || 'Stab';
            const currentAttr = w.attributeModifier || 'Strength';
            const dmgDisplay = dmgBonus !== 0
              ? `${w.damageDie}${dmgBonus > 0 ? `+${dmgBonus}` : dmgBonus}`
              : w.damageDie;

            const resolvedTags = (w.tags ?? '')
              .split(',')
              .map(c => tagByCode.get(c.trim()))
              .filter((t): t is LookupValue => t !== undefined);

            return (
              <div key={w.id} className="item-row" style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <div className="item-info">
                  <div className="item-name">{w.name}</div>
                  <div className="item-meta">
                    {dmgDisplay} dmg | Atk {atkBonus !== undefined ? (atkBonus >= 0 ? `+${atkBonus}` : atkBonus) : '?'}
                    {w.shockDamage !== null && (() => {
                      const shockBonus = derivedStats.weaponShockBonuses?.[w.id] ?? 0;
                      const totalShock = w.shockDamage + shockBonus;
                      return ` | Shock ${totalShock}/${w.isArmorPiercing ? 'AP' : w.shockAcThreshold}`;
                    })()}
                  </div>
                  {resolvedTags.length > 0 && (
                    <div className="weapon-tag-list">
                      {resolvedTags.map(tag => (
                        <WeaponTagBadge key={tag.id} tag={tag} />
                      ))}
                    </div>
                  )}
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
                      {ALL_ATTRIBUTES.map(attr => (
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

      {conditionalFoci.length > 0 && (
        <div style={{ marginTop: '0.75rem' }}>
          <div style={{ fontSize: '0.8rem', color: 'var(--text-muted)', marginBottom: '0.25rem' }}>
            ACTIVE CONDITIONS
          </div>
          {conditionalFoci.map(f => (
            <label key={f.id} style={{ display: 'flex', alignItems: 'center', gap: '0.4rem', fontSize: '0.875rem', cursor: 'pointer', marginBottom: '0.25rem' }}>
              <input
                type="checkbox"
                checked={f.conditionalActive}
                onChange={() => handleConditionalToggle(f.id, f.conditionalActive)}
              />
              {f.name}
            </label>
          ))}
        </div>
      )}
    </SectionCard>
  );
}
