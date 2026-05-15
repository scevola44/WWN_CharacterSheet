import { useState, useEffect, useMemo } from 'react';
import type { Spell, CreateSpellRequest } from '../../types/spell';
import type { CharacterDetail } from '../../types/character';
import { spellsApi } from '../../api/spellApi';
import { SpellForm } from './SpellForm';

const EMPTY_FORM: CreateSpellRequest = {
  name: '',
  spellLevel: 1,
  description: '',
  summary: '',
};

export function SpellDatabaseModal({ character, onLearn, onClose }: {
  character: CharacterDetail;
  onLearn: () => void;
  onClose: () => void;
}) {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [filterLevel, setFilterLevel] = useState<number | 'all'>('all');
  const [searchName, setSearchName] = useState('');
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);
  const [createForm, setCreateForm] = useState<CreateSpellRequest>(EMPTY_FORM);
  const [createError, setCreateError] = useState<string | null>(null);

  useEffect(() => {
    spellsApi.list().then(setSpells).finally(() => setLoading(false));
  }, []);

  const filteredSpells = useMemo(() => {
    const knownSpellIds = new Set(character.spellbook.map(s => s.spellId));
    let filtered = spells.filter(s => !knownSpellIds.has(s.id));
    if (filterLevel !== 'all') {
      filtered = filtered.filter(s => s.spellLevel === filterLevel);
    }
    if (searchName) {
      filtered = filtered.filter(s => s.name.toLowerCase().includes(searchName.toLowerCase()));
    }
    return filtered;
  }, [spells, filterLevel, searchName, character.spellbook]);

  const handleLearn = async (spellId: string) => {
    await spellsApi.learnSpell(character.id, spellId);
    onLearn();
  };

  const handleCreate = async () => {
    if (!createForm.name.trim() || !createForm.description.trim()) {
      setCreateError('Name and description are required.');
      return;
    }
    setCreateError(null);
    try {
      const created = await spellsApi.create({
        name: createForm.name,
        spellLevel: createForm.spellLevel,
        description: createForm.description,
        summary: createForm.summary || undefined,
      });
      setSpells(prev => [...prev, created]);
      setCreateForm(EMPTY_FORM);
      setCreating(false);
    } catch (e) {
      setCreateError(e instanceof Error ? e.message : 'Failed to create spell.');
    }
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: '600px', maxHeight: '80vh', overflow: 'auto', minWidth: '400px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2>Add Spell</h2>
          <button className="sm danger" onClick={onClose}>✕</button>
        </div>

        {creating ? (
          <div style={{ border: '1px solid var(--border)', borderRadius: '0.25rem', padding: '0.75rem', marginBottom: '1rem' }}>
            <h3 style={{ marginTop: 0 }}>New Custom Spell</h3>
            {createError && (
              <div style={{ color: 'var(--danger)', fontSize: '0.875rem', marginBottom: '0.5rem' }}>{createError}</div>
            )}
            <SpellForm
              values={createForm}
              onChange={setCreateForm}
              onSubmit={handleCreate}
              onCancel={() => { setCreating(false); setCreateError(null); }}
              submitLabel="Create & Add to Library"
            />
          </div>
        ) : (
          <div style={{ marginBottom: '1rem' }}>
            <button className="sm" onClick={() => setCreating(true)}>+ Create custom spell</button>
          </div>
        )}

        <div className="form-group">
          <label>Search Name</label>
          <input
            type="text"
            value={searchName}
            onChange={e => setSearchName(e.target.value)}
            placeholder="Filter by name..."
          />
        </div>

        <div className="form-group">
          <label>Level</label>
          <select value={filterLevel} onChange={e => setFilterLevel(e.target.value === 'all' ? 'all' : parseInt(e.target.value))}>
            <option value="all">All Levels</option>
            {[1, 2, 3, 4, 5, 6].map(l => <option key={l} value={l}>Level {l}</option>)}
          </select>
        </div>

        {loading ? (
          <div>Loading spells...</div>
        ) : filteredSpells.length === 0 ? (
          <div>No spells available to learn</div>
        ) : (
          <div style={{ marginTop: '1rem', display: 'grid', gap: '0.75rem' }}>
            {filteredSpells.map(spell => (
              <div key={spell.id} style={{ border: '1px solid var(--border)', padding: '0.75rem', borderRadius: '0.25rem' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontWeight: 'bold', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                      {spell.name}
                      {spell.isCustom && <CustomBadge />}
                    </div>
                    <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>Level {spell.spellLevel}</div>
                    {spell.summary && (
                      <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                        {spell.summary}
                      </div>
                    )}
                    <div style={{ fontSize: '0.875rem', marginTop: '0.25rem' }}>{spell.description}</div>
                  </div>
                  <button className="sm" onClick={() => handleLearn(spell.id)} style={{ marginLeft: '0.5rem' }}>Learn</button>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function CustomBadge() {
  return (
    <span style={{
      fontSize: '0.7rem',
      padding: '0.1rem 0.4rem',
      borderRadius: '0.2rem',
      background: 'var(--accent)',
      color: 'var(--bg)',
      fontWeight: 'normal',
    }}>
      Custom
    </span>
  );
}
