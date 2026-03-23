import type { CreateFocusDefinitionRequest, UpdateFocusDefinitionRequest } from '../../types/focusDefinition';

export function FocusForm({
  values,
  onChange,
  onSubmit,
  onCancel,
  submitLabel,
}: {
  values: CreateFocusDefinitionRequest | UpdateFocusDefinitionRequest;
  onChange: (v: CreateFocusDefinitionRequest | UpdateFocusDefinitionRequest) => void;
  onSubmit: () => void;
  onCancel: () => void;
  submitLabel: string;
}) {
  return (
    <>
      <div className="form-row">
        <div className="form-group" style={{ flex: 2 }}>
          <label>Name *</label>
          <input
            type="text"
            value={values.name}
            onChange={e => onChange({ ...values, name: e.target.value })}
            placeholder="Focus name"
          />
        </div>
        <div className="form-group" style={{ flex: 1, display: 'flex', alignItems: 'center', gap: '0.5rem', paddingTop: '1.5rem' }}>
          <input
            type="checkbox"
            id="canTakeMultiple"
            checked={values.canTakeMultipleTimes}
            onChange={e => onChange({ ...values, canTakeMultipleTimes: e.target.checked })}
          />
          <label htmlFor="canTakeMultiple" style={{ marginBottom: 0 }}>Can take multiple times</label>
        </div>
      </div>

      <div className="form-group">
        <label>Description</label>
        <input
          type="text"
          value={values.description || ''}
          onChange={e => onChange({ ...values, description: e.target.value })}
          placeholder="Brief overview of the focus"
        />
      </div>

      <div className="form-group">
        <label>Level 1 Description *</label>
        <textarea
          value={values.level1Description}
          onChange={e => onChange({ ...values, level1Description: e.target.value })}
          placeholder="What the character gains at level 1"
          rows={3}
        />
      </div>

      <div className="form-group">
        <label>Level 2 Description</label>
        <textarea
          value={values.level2Description || ''}
          onChange={e => onChange({ ...values, level2Description: e.target.value })}
          placeholder="What the character gains at level 2 (leave blank if this focus only has one level)"
          rows={3}
        />
      </div>

      <div style={{ display: 'flex', gap: '0.5rem' }}>
        <button onClick={onSubmit}>{submitLabel}</button>
        <button className="secondary" onClick={onCancel}>Cancel</button>
      </div>
    </>
  );
}
