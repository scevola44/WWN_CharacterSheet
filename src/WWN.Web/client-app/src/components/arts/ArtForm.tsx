import type { UpdateArtRequest } from '../../types/art';
import { useEffortCommitments, useArtSources } from '../../contexts/LookupsContext';

export function ArtForm({
  values,
  onChange,
  onSubmit,
  onCancel,
  submitLabel,
}: {
  values: UpdateArtRequest;
  onChange: (v: UpdateArtRequest) => void;
  onSubmit: () => void;
  onCancel: () => void;
  submitLabel: string;
}) {
  const effortOptions = useEffortCommitments();
  const sourceOptions = useArtSources();

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
          <label>Min Level *</label>
          <select
            value={values.minLevel}
            onChange={e => onChange({ ...values, minLevel: parseInt(e.target.value) })}
          >
            {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map(l => (
              <option key={l} value={l}>Level {l}</option>
            ))}
          </select>
        </div>
        <div className="form-group" style={{ flex: 1 }}>
          <label>Effort Cost</label>
          <select
            value={values.effortCost}
            onChange={e => onChange({ ...values, effortCost: parseInt(e.target.value) })}
          >
            {effortOptions.map(o => (
              <option key={o.id} value={o.id}>{o.displayName}</option>
            ))}
          </select>
        </div>
        <div className="form-group" style={{ flex: 1 }}>
          <label>Source</label>
          <select
            value={values.sourceId}
            onChange={e => onChange({ ...values, sourceId: parseInt(e.target.value) })}
          >
            {sourceOptions.map(o => (
              <option key={o.id} value={o.id}>{o.displayName}</option>
            ))}
          </select>
        </div>
      </div>
      <div className="form-group">
        <label>Summary</label>
        <input
          type="text"
          value={values.summary || ''}
          onChange={e => onChange({ ...values, summary: e.target.value })}
          placeholder="Brief summary"
        />
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
