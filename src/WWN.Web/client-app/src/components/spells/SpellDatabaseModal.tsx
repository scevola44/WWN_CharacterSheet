import { useState, useEffect } from 'react';
import type { Spell } from '../../types/spell';
import type { CharacterDetail } from '../../types/character';
import { spellsApi } from '../../api/spellApi';

export function SpellDatabaseModal({ character, onLearn, onClose }: {
  character: CharacterDetail;
  onLearn: () => void;
  onClose: () => void;
}) {
  const [spells, setSpells] = useState<Spell[]>([]);
  const [filteredSpells, setFilteredSpells] = useState<Spell[]>([]);
  const [filterLevel, setFilterLevel] = useState<number | 'all'>('all');
  const [searchName, setSearchName] = useState('');
  const [loading, setLoading] = useState(true);

  const knownSpellIds = new Set(character.spellbook.map(s => s.spellId));

  useEffect(() => {
    spellsApi.list().then(setSpells).finally(() => setLoading(false));
  }, []);

  useEffect(() => {
    let filtered = spells.filter(s => !knownSpellIds.has(s.id));

    if (filterLevel !== 'all') {
      filtered = filtered.filter(s => s.spellLevel === filterLevel);
    }
    if (searchName) {
      filtered = filtered.filter(s => s.name.toLowerCase().includes(searchName.toLowerCase()));
    }

    setFilteredSpells(filtered);
  }, [spells, filterLevel, searchName, knownSpellIds]);

  const handleLearn = async (spellId: string) => {
    await spellsApi.learnSpell(character.id, spellId);
    onLearn();
  };

  return (
    <div style={{ position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.7)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
      <div style={{ background: 'var(--bg-card)', padding: '1.5rem', borderRadius: '0.5rem', maxWidth: '600px', maxHeight: '80vh', overflow: 'auto', minWidth: '400px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1rem' }}>
          <h2>Add Spell</h2>
          <button className="sm danger" onClick={onClose}>✕</button>
        </div>

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
                    <div style={{ fontWeight: 'bold' }}>
                      {spell.name}
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
