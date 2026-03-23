import type { UpdateSpellRequest } from '../../types/spell';

export function SpellForm({
  values,
  onChange,
  onSubmit,
  onCancel,
  submitLabel,
}: {
  values: UpdateSpellRequest;
  onChange: (v: UpdateSpellRequest) => void;
  onSubmit: () => void;
  onCancel: () => void;
  submitLabel: string;
}) {
  return (
    <>
      <div className="form-group">
        <label>Name *</label>
        <input
          type="text"
          value={values.name}
          onChange={e => onChange({ ...values, name: e.target.value })}
        />
      </div>
      <div className="form-row">
        <div className="form-group" style={{ flex: 1 }}>
          <label>Level *</label>
          <select value={values.spellLevel} onChange={e => onChange({ ...values, spellLevel: parseInt(e.target.value) })}>
            {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
          </select>
        </div>
        <div className="form-group" style={{ flex: 1 }}>
          <label>Summary</label>
          <input
            type="text"
            value={values.summary || ''}
            onChange={e => onChange({ ...values, summary: e.target.value })}
            placeholder="Brief summary"
          />
        </div>
      </div>
      <div className="form-group">
        <label>Description *</label>
        <textarea
          value={values.description}
          onChange={e => onChange({ ...values, description: e.target.value })}
          rows={4}
        />
      </div>
      <div style={{ display: 'flex', gap: '0.5rem' }}>
        <button onClick={onSubmit}>{submitLabel}</button>
        <button className="secondary" onClick={onCancel}>Cancel</button>
      </div>
    </>
  );
}
