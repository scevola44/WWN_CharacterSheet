import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { characterApi } from '../api/characterApi';
import type { CreateCharacterRequest } from '../types/character';

const ATTRIBUTES = ['Strength', 'Dexterity', 'Constitution', 'Intelligence', 'Wisdom', 'Charisma'];
const CLASSES = ['Warrior', 'Expert', 'Mage', 'Adventurer'];
const PARTIAL_CLASSES = ['PartialWarrior', 'PartialExpert', 'PartialMage'];

export function CharacterCreatePage() {
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [charClass, setCharClass] = useState('Warrior');
  const [partialA, setPartialA] = useState('PartialWarrior');
  const [partialB, setPartialB] = useState('PartialExpert');
  const [background, setBackground] = useState('');
  const [origin, setOrigin] = useState('');
  const [scores, setScores] = useState<Record<string, number>>(
    Object.fromEntries(ATTRIBUTES.map(a => [a, 10]))
  );
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      const req: CreateCharacterRequest = {
        name,
        class: charClass,
        attributes: scores,
        background: background || undefined,
        origin: origin || undefined,
        partialClassA: charClass === 'Adventurer' ? partialA : undefined,
        partialClassB: charClass === 'Adventurer' ? partialB : undefined,
      };
      const id = await characterApi.create(req);
      navigate(`/character/${id}`);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to create character');
    }
  };

  return (
    <div>
      <h2>Create Character</h2>
      <form onSubmit={handleSubmit} style={{ maxWidth: 600 }}>
        <div className="section-card">
          <h2>Basic Info</h2>
          <div className="form-group">
            <label>Name *</label>
            <input value={name} onChange={e => setName(e.target.value)} required style={{ width: '100%' }} />
          </div>
          <div className="form-row">
            <div className="form-group">
              <label>Background</label>
              <input value={background} onChange={e => setBackground(e.target.value)} />
            </div>
            <div className="form-group">
              <label>Origin</label>
              <input value={origin} onChange={e => setOrigin(e.target.value)} />
            </div>
          </div>
          <div className="form-group">
            <label>Class *</label>
            <select value={charClass} onChange={e => setCharClass(e.target.value)}>
              {CLASSES.map(c => <option key={c}>{c}</option>)}
            </select>
          </div>
          {charClass === 'Adventurer' && (
            <div className="form-row">
              <div className="form-group">
                <label>Partial Class A</label>
                <select value={partialA} onChange={e => setPartialA(e.target.value)}>
                  {PARTIAL_CLASSES.map(c => <option key={c} value={c}>{c.replace('Partial', '')}</option>)}
                </select>
              </div>
              <div className="form-group">
                <label>Partial Class B</label>
                <select value={partialB} onChange={e => setPartialB(e.target.value)}>
                  {PARTIAL_CLASSES.map(c => <option key={c} value={c}>{c.replace('Partial', '')}</option>)}
                </select>
              </div>
            </div>
          )}
        </div>

        <div className="section-card">
          <h2>Attributes</h2>
          <div className="attr-grid">
            {ATTRIBUTES.map(attr => (
              <div key={attr} className="form-group" style={{ textAlign: 'center' }}>
                <label>{attr.substring(0, 3).toUpperCase()}</label>
                <input
                  type="number"
                  min={3}
                  max={18}
                  value={scores[attr]}
                  onChange={e => setScores({ ...scores, [attr]: +e.target.value })}
                />
              </div>
            ))}
          </div>
        </div>

        {error && <div className="error">{error}</div>}

        <div style={{ display: 'flex', gap: '0.5rem', marginTop: '1rem' }}>
          <button type="button" className="secondary" onClick={() => navigate('/')}>Cancel</button>
          <button type="submit">Create Character</button>
        </div>
      </form>
    </div>
  );
}
