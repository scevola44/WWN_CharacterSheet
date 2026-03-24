import type { FocusEffectTemplate } from '../../types/focusDefinition';

const EFFECT_TYPES = [
  { value: 'SkillBonus', label: 'Skill Bonus' },
  { value: 'AttributeBonus', label: 'Attribute Bonus' },
  { value: 'AttackBonus', label: 'Attack Bonus' },
  { value: 'DamageBonus', label: 'Damage Bonus' },
  { value: 'AcBonus', label: 'AC Bonus' },
  { value: 'ShockBonus', label: 'Shock Bonus' },
  { value: 'HpBonus', label: 'HP Bonus' },
  { value: 'SaveBonus', label: 'Save Bonus' },
  { value: 'Initiative', label: 'Initiative' },
  { value: 'Custom', label: 'Custom' },
];

const VALUE_TYPES = [
  { value: 'Static', label: 'Fixed number' },
  { value: 'Level', label: 'Character level' },
  { value: 'HalfLevel', label: 'Half character level' },
  { value: 'SkillLevel', label: 'Skill rank' },
];

const CONDITIONS = [
  { value: 'Always', label: 'Always' },
  { value: 'StabWeapon', label: 'Stab weapons (melee + thrown)' },
  { value: 'ShootWeapon', label: 'Shoot weapons (ranged)' },
  { value: 'PunchWeapon', label: 'Punch weapons (unarmed)' },
  { value: 'Conditional', label: 'Conditional (manual checkbox)' },
];

const SKILLS = [
  'Administer', 'Connect', 'Exert', 'Fix', 'Heal', 'Know', 'Lead',
  'Notice', 'Perform', 'Pilot', 'Punch', 'Ride', 'Sail', 'Shoot',
  'Sneak', 'Stab', 'Survive', 'Talk', 'Trade', 'Work',
];

const ATTRIBUTES = ['Strength', 'Dexterity', 'Constitution', 'Intelligence', 'Wisdom', 'Charisma'];

function newEffect(): FocusEffectTemplate {
  return { type: 'AttackBonus', numericValue: 1, valueType: 'Static', condition: 'Always' };
}

export function FocusEffectEditor({
  effects,
  onChange,
}: {
  effects: FocusEffectTemplate[];
  onChange: (effects: FocusEffectTemplate[]) => void;
}) {
  const update = (index: number, patch: Partial<FocusEffectTemplate>) => {
    const next = effects.map((e, i) => (i === index ? { ...e, ...patch } : e));
    onChange(next);
  };

  const remove = (index: number) => onChange(effects.filter((_, i) => i !== index));

  return (
    <div style={{ display: 'grid', gap: '0.5rem' }}>
      {effects.map((effect, i) => (
        <div
          key={i}
          style={{
            display: 'grid',
            gridTemplateColumns: 'auto auto auto 1fr auto',
            gap: '0.4rem',
            alignItems: 'center',
            padding: '0.4rem 0.5rem',
            background: 'var(--bg)',
            borderRadius: '0.25rem',
            border: '1px solid var(--border)',
            flexWrap: 'wrap',
          }}
        >
          {/* Type */}
          <select
            value={effect.type}
            onChange={e => update(i, { type: e.target.value, targetSkill: undefined, targetAttribute: undefined })}
            style={{ fontSize: '0.8rem', padding: '0.2rem' }}
          >
            {EFFECT_TYPES.map(t => <option key={t.value} value={t.value}>{t.label}</option>)}
          </select>

          {/* Value */}
          <div style={{ display: 'flex', gap: '0.25rem', alignItems: 'center' }}>
            <select
              value={effect.valueType}
              onChange={e => update(i, { valueType: e.target.value })}
              style={{ fontSize: '0.8rem', padding: '0.2rem' }}
            >
              {VALUE_TYPES.map(v => <option key={v.value} value={v.value}>{v.label}</option>)}
            </select>
            {effect.valueType === 'Static' && (
              <input
                type="number"
                value={effect.numericValue}
                onChange={e => update(i, { numericValue: parseInt(e.target.value) || 0 })}
                style={{ width: '3.5rem', fontSize: '0.8rem', padding: '0.2rem' }}
              />
            )}
            {effect.valueType === 'SkillLevel' && (
              <select
                value={effect.targetSkill || ''}
                onChange={e => update(i, { targetSkill: e.target.value || undefined })}
                style={{ fontSize: '0.8rem', padding: '0.2rem' }}
              >
                <option value="">Pick skill…</option>
                {SKILLS.map(s => <option key={s} value={s}>{s}</option>)}
              </select>
            )}
          </div>

          {/* Condition */}
          <select
            value={effect.condition}
            onChange={e => update(i, { condition: e.target.value })}
            style={{ fontSize: '0.8rem', padding: '0.2rem' }}
          >
            {CONDITIONS.map(c => <option key={c.value} value={c.value}>{c.label}</option>)}
          </select>

          {/* Contextual target fields */}
          <div style={{ display: 'flex', gap: '0.25rem' }}>
            {effect.type === 'SkillBonus' && (
              <select
                value={effect.targetSkill || ''}
                onChange={e => update(i, { targetSkill: e.target.value || undefined })}
                style={{ fontSize: '0.8rem', padding: '0.2rem' }}
              >
                <option value="">Target skill…</option>
                {SKILLS.map(s => <option key={s} value={s}>{s}</option>)}
              </select>
            )}
            {effect.type === 'AttributeBonus' && (
              <select
                value={effect.targetAttribute || ''}
                onChange={e => update(i, { targetAttribute: e.target.value || undefined })}
                style={{ fontSize: '0.8rem', padding: '0.2rem' }}
              >
                <option value="">Target attr…</option>
                {ATTRIBUTES.map(a => <option key={a} value={a}>{a}</option>)}
              </select>
            )}
            {effect.type === 'Custom' && (
              <input
                type="text"
                value={effect.description || ''}
                onChange={e => update(i, { description: e.target.value || undefined })}
                placeholder="Description…"
                style={{ fontSize: '0.8rem', padding: '0.2rem', minWidth: '8rem' }}
              />
            )}
          </div>

          <button
            className="sm danger"
            onClick={() => remove(i)}
            style={{ padding: '0.15rem 0.4rem', fontSize: '0.75rem' }}
          >
            ✕
          </button>
        </div>
      ))}
      <button
        className="sm secondary"
        onClick={() => onChange([...effects, newEffect()])}
        style={{ alignSelf: 'start' }}
      >
        + Add effect
      </button>
    </div>
  );
}
