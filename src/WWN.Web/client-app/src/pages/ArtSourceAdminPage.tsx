import { useState } from 'react';
import type { LookupValue } from '../types/lookups';
import { artSourceApi } from '../api/artSourceApi';
import type { CreateArtSourceRequest, UpdateArtSourceRequest } from '../api/artSourceApi';
import { useArtSources, useLookups } from '../contexts/LookupsContext';

type EditState = { id: number; displayName: string; description: string; sortOrder: number };
type AddState = { code: string; displayName: string; description: string; sortOrder: number };

const emptyAdd: AddState = { code: '', displayName: '', description: '', sortOrder: 0 };

export function ArtSourceAdminPage() {
  const sources = useArtSources();
  const { refresh } = useLookups();
  const [editing, setEditing] = useState<EditState | null>(null);
  const [adding, setAdding] = useState(false);
  const [addForm, setAddForm] = useState<AddState>(emptyAdd);
  const [error, setError] = useState<string | null>(null);

  const handleAdd = async () => {
    if (!addForm.code.trim() || !addForm.displayName.trim()) {
      setError('Code and Display Name are required.');
      return;
    }
    setError(null);
    try {
      const req: CreateArtSourceRequest = {
        code: addForm.code.trim(),
        displayName: addForm.displayName.trim(),
        description: addForm.description.trim() || undefined,
        sortOrder: addForm.sortOrder,
      };
      await artSourceApi.create(req);
      setAddForm(emptyAdd);
      setAdding(false);
      refresh();
    } catch (e: unknown) {
      const msg = (e as { response?: { data?: unknown } })?.response?.data;
      setError(typeof msg === 'string' ? msg : 'Failed to create art source.');
    }
  };

  const handleEditSave = async () => {
    if (!editing) return;
    setError(null);
    try {
      const req: UpdateArtSourceRequest = {
        displayName: editing.displayName.trim(),
        description: editing.description.trim() || undefined,
        sortOrder: editing.sortOrder,
      };
      await artSourceApi.update(editing.id, req);
      setEditing(null);
      refresh();
    } catch {
      setError('Failed to update art source.');
    }
  };

  const handleDelete = async (source: LookupValue) => {
    if (!confirm(`Delete "${source.displayName}"?`)) return;
    setError(null);
    try {
      await artSourceApi.delete(source.id);
      refresh();
    } catch (e: unknown) {
      const status = (e as { response?: { status?: number } })?.response?.status;
      if (status === 409) {
        setError(`Cannot delete "${source.displayName}" — it is referenced by one or more arts.`);
      } else {
        setError('Failed to delete art source.');
      }
    }
  };

  const startEdit = (s: LookupValue) => {
    setEditing({ id: s.id, displayName: s.displayName, description: s.description ?? '', sortOrder: s.sortOrder });
    setAdding(false);
    setError(null);
  };

  return (
    <div className="page-content">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Art Sources</h1>
        <button onClick={() => { setAdding(true); setEditing(null); setError(null); }}>+ Add Source</button>
      </div>

      {error && (
        <div role="alert" style={{ color: 'var(--danger)', background: 'var(--surface)', padding: '0.75rem 1rem', borderRadius: '0.375rem', marginBottom: '1rem', border: '1px solid var(--danger)' }}>
          {error}
        </div>
      )}

      {adding && (
        <div style={{ background: 'var(--surface)', padding: '1.5rem', borderRadius: '0.5rem', marginBottom: '1.5rem', border: '1px solid var(--border)' }}>
          <h2>Add Art Source</h2>
          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>Code *</label>
              <input
                type="text"
                value={addForm.code}
                onChange={e => setAddForm(f => ({ ...f, code: e.target.value }))}
                placeholder="e.g. PartialMage"
              />
            </div>
            <div className="form-group" style={{ flex: 1 }}>
              <label>Display Name *</label>
              <input
                type="text"
                value={addForm.displayName}
                onChange={e => setAddForm(f => ({ ...f, displayName: e.target.value }))}
                placeholder="e.g. Partial Mage"
              />
            </div>
            <div className="form-group" style={{ flex: 0, minWidth: '6rem' }}>
              <label>Sort Order</label>
              <input
                type="number"
                value={addForm.sortOrder}
                onChange={e => setAddForm(f => ({ ...f, sortOrder: parseInt(e.target.value) || 0 }))}
              />
            </div>
          </div>
          <div className="form-group">
            <label>Description</label>
            <input
              type="text"
              value={addForm.description}
              onChange={e => setAddForm(f => ({ ...f, description: e.target.value }))}
            />
          </div>
          <div style={{ display: 'flex', gap: '0.5rem' }}>
            <button onClick={handleAdd}>Add</button>
            <button className="secondary" onClick={() => { setAdding(false); setError(null); }}>Cancel</button>
          </div>
        </div>
      )}

      <table style={{ width: '100%', borderCollapse: 'collapse' }}>
        <thead>
          <tr style={{ borderBottom: '1px solid var(--border)' }}>
            <th style={{ textAlign: 'left', padding: '0.5rem' }}>Display Name</th>
            <th style={{ textAlign: 'left', padding: '0.5rem' }}>Code</th>
            <th style={{ textAlign: 'left', padding: '0.5rem' }}>Sort</th>
            <th style={{ textAlign: 'left', padding: '0.5rem' }}>Description</th>
            <th style={{ padding: '0.5rem' }}></th>
          </tr>
        </thead>
        <tbody>
          {sources.map(s => (
            <tr key={s.id} style={{ borderBottom: '1px solid var(--border)' }}>
              {editing?.id === s.id ? (
                <>
                  <td style={{ padding: '0.5rem' }}>
                    <input
                      type="text"
                      value={editing.displayName}
                      onChange={e => setEditing(ed => ed ? { ...ed, displayName: e.target.value } : ed)}
                      style={{ width: '100%' }}
                    />
                  </td>
                  <td style={{ padding: '0.5rem', color: 'var(--text-muted)', fontSize: '0.875rem' }}>{s.code}</td>
                  <td style={{ padding: '0.5rem' }}>
                    <input
                      type="number"
                      value={editing.sortOrder}
                      onChange={e => setEditing(ed => ed ? { ...ed, sortOrder: parseInt(e.target.value) || 0 } : ed)}
                      style={{ width: '4rem' }}
                    />
                  </td>
                  <td style={{ padding: '0.5rem' }}>
                    <input
                      type="text"
                      value={editing.description}
                      onChange={e => setEditing(ed => ed ? { ...ed, description: e.target.value } : ed)}
                      style={{ width: '100%' }}
                    />
                  </td>
                  <td style={{ padding: '0.5rem', display: 'flex', gap: '0.25rem' }}>
                    <button className="sm" onClick={handleEditSave}>Save</button>
                    <button className="sm secondary" onClick={() => setEditing(null)}>Cancel</button>
                  </td>
                </>
              ) : (
                <>
                  <td style={{ padding: '0.5rem' }}>{s.displayName}</td>
                  <td style={{ padding: '0.5rem', color: 'var(--text-muted)', fontSize: '0.875rem' }}>{s.code}</td>
                  <td style={{ padding: '0.5rem' }}>{s.sortOrder}</td>
                  <td style={{ padding: '0.5rem', color: 'var(--text-muted)', fontSize: '0.875rem' }}>{s.description ?? '—'}</td>
                  <td style={{ padding: '0.5rem', display: 'flex', gap: '0.25rem' }}>
                    <button className="sm" onClick={() => startEdit(s)}>Edit</button>
                    <button className="sm danger" onClick={() => handleDelete(s)}>Delete</button>
                  </td>
                </>
              )}
            </tr>
          ))}
          {sources.length === 0 && (
            <tr>
              <td colSpan={5} style={{ padding: '1rem', textAlign: 'center', color: 'var(--text-muted)' }}>
                No art sources defined.
              </td>
            </tr>
          )}
        </tbody>
      </table>
    </div>
  );
}
