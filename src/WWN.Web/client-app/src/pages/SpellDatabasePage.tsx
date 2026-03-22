import { useEffect, useState } from 'react';
import type { Spell, CreateSpellRequest } from '../types/spell';
import { spellsApi } from '../api/spellApi';

export function SpellDatabasePage() {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [filterLevel, setFilterLevel] = useState<number | 'all'>('all');
  const [filterSchool, setFilterSchool] = useState<string>('');
  const [form, setForm] = useState<CreateSpellRequest>({
    name: '',
    spellLevel: 1,
    description: '',
    school: '',
    duration: '',
    range: '',
  });

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
      school: form.school || undefined,
      duration: form.duration || undefined,
      range: form.range || undefined,
    };

    await spellsApi.create(req);
    setForm({ name: '', spellLevel: 1, description: '', school: '', duration: '', range: '' });
    setShowForm(false);
    refreshSpells();
  };

  const handleDelete = async (id: string) => {
    if (confirm('Delete this spell?')) {
      await spellsApi.delete(id);
      refreshSpells();
    }
  };

  const schools = [...new Set(spells.map(s => s.school).filter(Boolean))].sort();
  const filteredSpells = spells.filter(s => {
    if (filterLevel !== 'all' && s.spellLevel !== filterLevel) return false;
    if (filterSchool && s.school !== filterSchool) return false;
    return true;
  });

  return (
    <div style={{ maxWidth: '1000px', margin: '0 auto', padding: '1rem' }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Spell Database</h1>
        <button onClick={() => setShowForm(!showForm)}>+ Add Spell</button>
      </div>

      {showForm && (
        <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', marginBottom: '1.5rem', border: '1px solid var(--border)' }}>
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
              <label>School</label>
              <input
                type="text"
                value={form.school || ''}
                onChange={e => setForm({ ...form, school: e.target.value })}
                placeholder="e.g., Evocation"
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

          <div className="form-group">
            <label>Duration</label>
            <input
              type="text"
              value={form.duration || ''}
              onChange={e => setForm({ ...form, duration: e.target.value })}
              placeholder="e.g., 1 hour"
            />
          </div>

          <div className="form-group">
            <label>Range</label>
            <input
              type="text"
              value={form.range || ''}
              onChange={e => setForm({ ...form, range: e.target.value })}
              placeholder="e.g., 60 feet"
            />
          </div>

          <div style={{ display: 'flex', gap: '0.5rem' }}>
            <button onClick={handleAddSpell}>Add Spell</button>
            <button className="secondary" onClick={() => setShowForm(false)}>Cancel</button>
          </div>
        </div>
      )}

      <div style={{ display: 'flex', gap: '1rem', marginBottom: '1.5rem' }}>
        <div>
          <label>Filter by Level</label>
          <select value={filterLevel} onChange={e => setFilterLevel(e.target.value === 'all' ? 'all' : parseInt(e.target.value))}>
            <option value="all">All Levels</option>
            {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
          </select>
        </div>
        <div>
          <label>Filter by School</label>
          <select value={filterSchool ?? ''} onChange={e => setFilterSchool(e.target.value)}>
            <option value="">All Schools</option>
            {schools.map(s => <option key={s ?? ''} value={s ?? ''}>{s}</option>)}
          </select>
        </div>
      </div>

      {loading ? (
        <div>Loading spells...</div>
      ) : filteredSpells.length === 0 ? (
        <div style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
          No spells yet. Add one to get started!
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '1rem' }}>
          {filteredSpells.map(spell => (
            <div key={spell.id} style={{ background: 'var(--bg-card)', padding: '1rem', borderRadius: '0.5rem', border: '1px solid var(--border)' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '0.5rem' }}>
                <div>
                  <h3 style={{ margin: 0 }}>
                    {spell.name}
                    {spell.school && <span style={{ fontSize: '0.75rem', marginLeft: '0.5rem', color: 'var(--primary)' }}>({spell.school})</span>}
                  </h3>
                  <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>
                    Level {spell.spellLevel}
                    {spell.range && ` | Range: ${spell.range}`}
                    {spell.duration && ` | Duration: ${spell.duration}`}
                  </div>
                </div>
                <button className="sm danger" onClick={() => handleDelete(spell.id)}>Delete</button>
              </div>
              <p style={{ margin: '0.5rem 0 0 0', fontSize: '0.875rem' }}>{spell.description}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
