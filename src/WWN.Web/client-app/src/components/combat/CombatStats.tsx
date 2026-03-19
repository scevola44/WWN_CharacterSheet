import { SectionCard } from '../layout/SectionCard';
import type { CharacterDetail } from '../../types/character';

export function CombatStats({ character }: { character: CharacterDetail }) {
  const { derivedStats } = character;

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
            return (
              <div key={w.id} className="item-row">
                <div className="item-info">
                  <div className="item-name">{w.name}</div>
                  <div className="item-meta">
                    {w.damageDie} dmg | Atk {atkBonus !== undefined ? (atkBonus >= 0 ? `+${atkBonus}` : atkBonus) : '?'}
                    {w.shockDamage !== null && ` | Shock ${w.shockDamage}/${w.shockAcThreshold}`}
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
