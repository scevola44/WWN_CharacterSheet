import { useEffect, useState, useCallback } from 'react';
import type { Art, CreateArtRequest, UpdateArtRequest } from '../types/art';
import { artsApi } from '../api/artApi';
import { ArtForm } from '../components/arts/ArtForm';

export function ArtDatabasePage() {
  const [arts, setArts] = useState<Art[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [filterEffort, setFilterEffort] = useState<string>('all');
  const [editingArtId, setEditingArtId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateArtRequest>({
    name: '',
    description: '',
    summary: '',
    minLevel: 1,
    effortCost: null,
    source: 'Mage',
  });
  const [editForm, setEditForm] = useState<UpdateArtRequest | null>(null);

  // Initial load: loading starts true; no synchronous setState in the effect body.
  useEffect(() => {
    artsApi.list().then(setArts).finally(() => setLoading(false));
  }, []);

  const refreshArts = useCallback(async () => {
    setLoading(true);
    const data = await artsApi.list();
    setArts(data);
    setLoading(false);
  }, []);

  const handleAddArt = async () => {
    if (!form.name.trim() || !form.description.trim()) {
      alert('Name and description are required');
      return;
    }

    await artsApi.create({
      ...form,
      summary: form.summary || undefined,
    });
    setForm({ name: '', description: '', summary: '', minLevel: 1, effortCost: null, source: 'Mage' });
    setShowForm(false);
    refreshArts();
  };

  const handleEditStart = (art: Art) => {
    setEditingArtId(art.id);
    setEditForm({
      name: art.name,
      description: art.description,
      summary: art.summary || undefined,
      minLevel: art.minLevel,
      effortCost: art.effortCost,
      source: art.source,
    });
  };

  const handleEditSave = async (id: string) => {
    if (!editForm || !editForm.name.trim() || !editForm.description.trim()) {
      alert('Name and description are required');
      return;
    }

    await artsApi.update(id, editForm);
    setEditingArtId(null);
    setEditForm(null);
    refreshArts();
  };

  const handleEditCancel = () => {
    setEditingArtId(null);
    setEditForm(null);
  };

  const handleDelete = async (id: string) => {
    if (confirm('Delete this art?')) {
      await artsApi.delete(id);
      refreshArts();
    }
  };

  const filteredArts = arts.filter(a => {
    if (filterEffort === 'all') return true;
    if (filterEffort === 'none') return a.effortCost == null;
    return a.effortCost === filterEffort;
  });

  return (
    <div className="page-content">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Art Database</h1>
        <button onClick={() => setShowForm(!showForm)}>+ Add Art</button>
      </div>

      {showForm && (
        <div style={{ background: 'var(--surface)', padding: '1.5rem', borderRadius: '0.5rem', marginBottom: '1.5rem', border: '1px solid var(--border)' }}>
          <h2>Add New Art</h2>
          <ArtForm
            values={form}
            onChange={v => setForm(v as CreateArtRequest)}
            onSubmit={handleAddArt}
            onCancel={() => setShowForm(false)}
            submitLabel="Add Art"
          />
        </div>
      )}

      <div>
        <label>Filter by Effort</label>
        <select value={filterEffort} onChange={e => setFilterEffort(e.target.value)}>
          <option value="all">Any cost</option>
          <option value="none">No effort</option>
          <option value="Scene">Scene</option>
          <option value="Day">Day</option>
          <option value="Sustained">Sustained</option>
        </select>
      </div>

      {loading ? (
        <div>Loading arts...</div>
      ) : filteredArts.length === 0 ? (
        <div style={{ color: 'var(--text-muted)', textAlign: 'center', padding: '2rem' }}>
          No arts yet. Add one to get started!
        </div>
      ) : (
        <div style={{ display: 'grid', gap: '1rem', marginTop: '1.5rem' }}>
          {filteredArts.map(art => (
            <div key={art.id} style={{ background: 'var(--surface)', padding: '1rem', borderRadius: '0.5rem', border: '1px solid var(--border)' }}>
              {editingArtId === art.id && editForm ? (
                <div>
                  <h3 style={{ margin: '0 0 1rem 0' }}>Edit Art</h3>
                  <ArtForm
                    values={editForm}
                    onChange={setEditForm}
                    onSubmit={() => handleEditSave(art.id)}
                    onCancel={handleEditCancel}
                    submitLabel="Save"
                  />
                </div>
              ) : (
                <>
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start', marginBottom: '0.5rem' }}>
                    <div>
                      <h3 style={{ margin: 0 }}>{art.name}</h3>
                      <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)' }}>
                        Min Level {art.minLevel} · {art.effortCost ? `Effort: ${art.effortCost}` : 'No effort'} · {art.source}
                      </div>
                      {art.summary && (
                        <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                          {art.summary}
                        </div>
                      )}
                    </div>
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                      <button className="sm" onClick={() => handleEditStart(art)}>Edit</button>
                      <button className="sm danger" onClick={() => handleDelete(art.id)}>Delete</button>
                    </div>
                  </div>
                  <p style={{ margin: '0.5rem 0 0 0', fontSize: '0.875rem' }}>{art.description}</p>
                </>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
