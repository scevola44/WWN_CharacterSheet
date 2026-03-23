import { useEffect, useState } from 'react';
import type {
  FocusDefinition,
  CreateFocusDefinitionRequest,
  UpdateFocusDefinitionRequest,
} from '../types/focusDefinition';
import { focusDefinitionApi } from '../api/focusDefinitionApi';
import { FocusForm } from '../components/foci/FocusForm';

const emptyForm: CreateFocusDefinitionRequest = {
  name: '',
  description: '',
  level1Description: '',
  level2Description: '',
  canTakeMultipleTimes: false,
};

export function FocusDatabasePage() {
  const [foci, setFoci] = useState<FocusDefinition[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [filterText, setFilterText] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateFocusDefinitionRequest>(emptyForm);
  const [editForm, setEditForm] = useState<UpdateFocusDefinitionRequest | null>(null);

  useEffect(() => {
    refresh();
  }, []);

  const refresh = async () => {
    setLoading(true);
    const data = await focusDefinitionApi.list();
    setFoci(data);
    setLoading(false);
  };

  const handleAdd = async () => {
    if (!form.name.trim() || !form.level1Description.trim()) {
      alert('Name and Level 1 description are required.');
      return;
    }
    await focusDefinitionApi.create({
      name: form.name.trim(),
      description: form.description?.trim() || undefined,
      level1Description: form.level1Description.trim(),
      level2Description: form.level2Description?.trim() || undefined,
      canTakeMultipleTimes: form.canTakeMultipleTimes,
    });
    setForm(emptyForm);
    setShowForm(false);
    refresh();
  };

  const handleEditStart = (fd: FocusDefinition) => {
    setEditingId(fd.id);
    setEditForm({
      name: fd.name,
      description: fd.description || '',
      level1Description: fd.level1Description,
      level2Description: fd.level2Description || '',
      canTakeMultipleTimes: fd.canTakeMultipleTimes,
    });
  };

  const handleEditSave = async (id: string) => {
    if (!editForm || !editForm.name.trim() || !editForm.level1Description.trim()) {
      alert('Name and Level 1 description are required.');
      return;
    }
    await focusDefinitionApi.update(id, {
      name: editForm.name.trim(),
      description: editForm.description?.trim() || undefined,
      level1Description: editForm.level1Description.trim(),
      level2Description: editForm.level2Description?.trim() || undefined,
      canTakeMultipleTimes: editForm.canTakeMultipleTimes,
    });
    setEditingId(null);
    setEditForm(null);
    refresh();
  };

  const handleEditCancel = () => {
    setEditingId(null);
    setEditForm(null);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Delete this focus definition?')) {
      await focusDefinitionApi.delete(id);
      refresh();
    }
  };

  const filtered = foci.filter(f =>
    !filterText || f.name.toLowerCase().includes(filterText.toLowerCase())
  );

  return (
    <div className="page-content">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Foci Database</h1>
        <button onClick={() => setShowForm(!showForm)}>+ Add Focus</button>
      </div>

      {showForm && (
        <div style={{ background: 'var(--bg)', padding: '1.5rem', borderRadius: '0.5rem', marginBottom: '1.5rem', border: '1px solid var(--border)' }}>
          <h2>Add New Focus</h2>
          <FocusForm
            values={form}
            onChange={v => setForm(v as CreateFocusDefinitionRequest)}
            onSubmit={handleAdd}
            onCancel={() => setShowForm(false)}
            submitLabel="Add Focus"
          />
        </div>
      )}

      <div className="form-group" style={{ maxWidth: '300px' }}>
        <label>Search</label>
        <input
          type="text"
          value={filterText}
          onChange={e => setFilterText(e.target.value)}
          placeholder="Filter by name..."
        />
      </div>

      {loading ? (
        <div>Loading foci...</div>
      ) : filtered.length === 0 ? (
        <div style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
          No foci found.
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '1rem', marginTop: '1.5rem' }}>
          {filtered.map(fd => (
            <div key={fd.id} style={{ background: 'var(--surface)', padding: '1rem', borderRadius: '0.5rem', border: '1px solid var(--border)' }}>
              {editingId === fd.id && editForm ? (
                <div>
                  <h3 style={{ margin: '0 0 1rem 0' }}>Edit Focus</h3>
                  <FocusForm
                    values={editForm}
                    onChange={v => setEditForm(v as UpdateFocusDefinitionRequest)}
                    onSubmit={() => handleEditSave(fd.id)}
                    onCancel={handleEditCancel}
                    submitLabel="Save"
                  />
                </div>
              ) : (
                <FocusCard fd={fd} onEdit={() => handleEditStart(fd)} onDelete={() => handleDelete(fd.id)} />
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}


function FocusCard({ fd, onEdit, onDelete }: {
  fd: FocusDefinition;
  onEdit: () => void;
  onDelete: () => void;
}) {
  return (
    <>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '0.5rem' }}>
        <div>
          <h3 style={{ margin: 0 }}>{fd.name}</h3>
          <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem', display: 'flex', gap: '0.75rem' }}>
            {fd.hasLevel2 ? <span>Levels 1–2</span> : <span>Level 1 only</span>}
            {fd.canTakeMultipleTimes && <span style={{ color: 'var(--accent)' }}>Can take multiple times</span>}
          </div>
          {fd.description && (
            <p style={{ margin: '0.5rem 0 0 0', fontSize: '0.875rem', color: 'var(--text-muted)', fontStyle: 'italic' }}>
              {fd.description}
            </p>
          )}
        </div>
        <div style={{ display: 'flex', gap: '0.5rem', flexShrink: 0, marginLeft: '0.5rem' }}>
          <button className="sm" onClick={onEdit}>Edit</button>
          <button className="sm danger" onClick={onDelete}>Delete</button>
        </div>
      </div>

      <div style={{ marginTop: '0.75rem', display: 'grid', gap: '0.5rem' }}>
        <div style={{ background: 'var(--bg)', padding: '0.75rem', borderRadius: '0.25rem', border: '1px solid var(--border)' }}>
          <div style={{ fontSize: '0.75rem', fontWeight: 'bold', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '0.25rem' }}>
            Level 1
          </div>
          <p style={{ margin: 0, fontSize: '0.875rem' }}>{fd.level1Description}</p>
        </div>

        {fd.hasLevel2 && fd.level2Description && (
          <div style={{ background: 'var(--bg)', padding: '0.75rem', borderRadius: '0.25rem', border: '1px solid var(--border)' }}>
            <div style={{ fontSize: '0.75rem', fontWeight: 'bold', color: 'var(--text-muted)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '0.25rem' }}>
              Level 2
            </div>
            <p style={{ margin: 0, fontSize: '0.875rem' }}>{fd.level2Description}</p>
          </div>
        )}
      </div>
    </>
  );
}
