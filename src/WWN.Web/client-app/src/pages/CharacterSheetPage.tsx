import { useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useCharacter } from '../hooks/useCharacter';
import { IdentitySection } from '../components/identity/IdentitySection';
import { AttributePanel } from '../components/attributes/AttributePanel';
import { SkillPanel } from '../components/skills/SkillPanel';
import { HitPointTracker } from '../components/combat/HitPointTracker';
import { StrainTracker } from '../components/combat/StrainTracker';
import { CombatStats } from '../components/combat/CombatStats';
import { FociPanel } from '../components/foci/FociPanel';
import { InventoryPanel } from '../components/inventory/InventoryPanel';
import { SpellsPanel } from '../components/spells/SpellsPanel';
import { NotesSection } from '../components/notes/NotesSection';
import { ClassAbilitiesPanel } from '../components/abilities/ClassAbilitiesPanel';

export function CharacterSheetPage() {
  const { id } = useParams<{ id: string }>();
  const { character, setCharacter, loading, error } = useCharacter(id);
  const [isEditing, setIsEditing] = useState(false);

  if (loading) return <div className="loading">Loading character...</div>;
  if (error || !character) return <div className="error">{error || 'Character not found'}</div>;

  const handleUpdate = (updated: typeof character) => setCharacter(updated);

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Link to="/" style={{ color: 'var(--text-muted)', textDecoration: 'none', fontSize: '0.9rem' }}>
          &larr; Back to Characters
        </Link>
        <button
          onClick={() => setIsEditing(e => !e)}
          className={isEditing ? 'primary' : 'secondary'}
          style={{ minWidth: '5rem' }}
        >
          {isEditing ? 'Done' : 'Edit'}
        </button>
      </div>

      <div className="sheet-full" style={{ marginTop: '0.5rem' }}>
        <IdentitySection character={character} />
      </div>

      <div className="sheet-columns">
        <div className="sheet-col">
          <AttributePanel character={character} onUpdate={handleUpdate} isEditing={isEditing} />
          <CombatStats character={character} onUpdate={handleUpdate} />
          <HitPointTracker character={character} onUpdate={handleUpdate} />
          <StrainTracker character={character} onUpdate={handleUpdate} />
          <ClassAbilitiesPanel character={character} />
          <FociPanel character={character} onUpdate={handleUpdate} />
        </div>
        <div className="sheet-col">
          <SkillPanel character={character} onUpdate={handleUpdate} isEditing={isEditing} />
          <InventoryPanel character={character} onUpdate={handleUpdate} />
        </div>
      </div>

      <div className="sheet-full">
        <SpellsPanel character={character} onUpdate={handleUpdate} />
      </div>

      <NotesSection character={character} onUpdate={handleUpdate} />
    </div>
  );
}
