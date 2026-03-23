import { useEffect, useState } from 'react';
import type { Spell, CreateSpellRequest, UpdateSpellRequest } from '../types/spell';
import { spellsApi } from '../api/spellApi';

export function SpellDatabasePage() {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [filterLevel, setFilterLevel] = useState<number | 'all'>('all');
  const [editingSpellId, setEditingSpellId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateSpellRequest>({
    name: '',
    spellLevel: 1,
    description: '',
    summary: '',
  });
  const [editForm, setEditForm] = useState<UpdateSpellRequest | null>(null);

  useEffect(() => {
    refreshSpells();
  }, []);

  const refreshSpells = async () => {
    setLoading(true);
    const data = await spellsApi.list();
    setSpells(data);
    setLoading(false);
  };

  const handleAddSpell = async () => {
    if (!form.name.trim() || !form.description.trim()) {
      alert('Name and description are required');
      return;
    }

    const req: CreateSpellRequest = {
      name: form.name,
      spellLevel: form.spellLevel,
      description: form.description,
      summary: form.summary || undefined,
    };

    await spellsApi.create(req);
    setForm({ name: '', spellLevel: 1, description: '', summary: '' });
    setShowForm(false);
    refreshSpells();
  };

  const handleEditStart = (spell: Spell) => {
    setEditingSpellId(spell.id);
    setEditForm({
      name: spell.name,
      spellLevel: spell.spellLevel,
      description: spell.description,
      summary: spell.summary || undefined,
    });
  };

  const handleEditSave = async (id: string) => {
    if (!editForm || !editForm.name.trim() || !editForm.description.trim()) {
      alert('Name and description are required');
      return;
    }

    await spellsApi.update(id, editForm);
    setEditingSpellId(null);
    setEditForm(null);
    refreshSpells();
  };

  const handleEditCancel = () => {
    setEditingSpellId(null);
    setEditForm(null);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Delete this spell?')) {
      await spellsApi.delete(id);
      refreshSpells();
    }
  };

  const filteredSpells = spells.filter(s => {
    if (filterLevel !== 'all' && s.spellLevel !== filterLevel) return false;
    return true;
  });

  return (
    <div className="page-content">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Spell Database</h1>
        <button onClick={() => setShowForm(!showForm)}>+ Add Spell</button>
      </div>

      {showForm && (
        <div style={{ background: 'var(--surface)', padding: '1.5rem', borderRadius: '0.5rem', marginBottom: '1.5rem', border: '1px solid var(--border)' }}>
          <h2>Add New Spell</h2>
          <div className="form-group">
            <label>Name *</label>
            <input
              type="text"
              value={form.name}
              onChange={e => setForm({ ...form, name: e.target.value })}
              placeholder="Spell name"
            />
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>Level *</label>
              <select value={form.spellLevel} onChange={e => setForm({ ...form, spellLevel: parseInt(e.target.value) })}>
                {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
              </select>
            </div>
            <div className="form-group" style={{ flex: 1 }}>
              <label>Summary</label>
              <input
                type="text"
                value={form.summary || ''}
                onChange={e => setForm({ ...form, summary: e.target.value })}
                placeholder="Brief summary for listing"
              />
            </div>
          </div>

          <div className="form-group">
            <label>Description *</label>
            <textarea
              value={form.description}
              onChange={e => setForm({ ...form, description: e.target.value })}
              placeholder="Spell description"
              rows={4}
            />
          </div>

          <div style={{ display: 'flex', gap: '0.5rem' }}>
            <button onClick={handleAddSpell}>Add Spell</button>
            <button className="secondary" onClick={() => setShowForm(false)}>Cancel</button>
          </div>
        </div>
      )}

      <div>
        <label>Filter by Level</label>
        <select value={filterLevel} onChange={e => setFilterLevel(e.target.value === 'all' ? 'all' : parseInt(e.target.value))}>
          <option value="all">All Levels</option>
          {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
        </select>
      </div>

      {loading ? (
        <div>Loading spells...</div>
      ) : filteredSpells.length === 0 ? (
        <div style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
          No spells yet. Add one to get started!
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '1rem', marginTop: '1.5rem' }}>
          {filteredSpells.map(spell => (
            <div key={spell.id} style={{ background: 'var(--surface)', padding: '1rem', borderRadius: '0.5rem', border: '1px solid var(--border)' }}>
              {editingSpellId === spell.id && editForm ? (
                <div>
                  <h3 style={{ margin: '0 0 1rem 0' }}>Edit Spell</h3>
                  <div className="form-group">
                    <label>Name *</label>
                    <input
                      type="text"
                      value={editForm.name}
                      onChange={e => setEditForm({ ...editForm, name: e.target.value })}
                    />
                  </div>
                  <div className="form-row">
                    <div className="form-group" style={{ flex: 1 }}>
                      <label>Level *</label>
                      <select value={editForm.spellLevel} onChange={e => setEditForm({ ...editForm, spellLevel: parseInt(e.target.value) })}>
                        {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
                      </select>
                    </div>
                    <div className="form-group" style={{ flex: 1 }}>
                      <label>Summary</label>
                      <input
                        type="text"
                        value={editForm.summary || ''}
                        onChange={e => setEditForm({ ...editForm, summary: e.target.value })}
                        placeholder="Brief summary"
                      />
                    </div>
                  </div>
                  <div className="form-group">
                    <label>Description *</label>
                    <textarea
                      value={editForm.description}
                      onChange={e => setEditForm({ ...editForm, description: e.target.value })}
                      rows={4}
                    />
                  </div>
                  <div style={{ display: 'flex', gap: '0.5rem' }}>
                    <button onClick={() => handleEditSave(spell.id)}>Save</button>
                    <button className="secondary" onClick={handleEditCancel}>Cancel</button>
                  </div>
                </div>
              ) : (
                <>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '0.5rem' }}>
                    <div>
                      <h3 style={{ margin: 0 }}>{spell.name}</h3>
                      <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>Level {spell.spellLevel}</div>
                      {spell.summary && (
                        <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                          {spell.summary}
                        </div>
                      )}
                    </div>
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                      <button className="sm" onClick={() => handleEditStart(spell)}>Edit</button>
                      <button className="sm danger" onClick={() => handleDelete(spell.id)}>Delete</button>
                    </div>
                  </div>
                  <p style={{ margin: '0.5rem 0 0 0', fontSize: '0.875rem' }}>{spell.description}</p>
                </>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
