import { useEffect, useState, useCallback } from 'react';
import type { Art, CreateArtRequest, UpdateArtRequest } from '../types/art';
import { artsApi } from '../api/artApi';
import { ArtForm } from '../components/arts/ArtForm';
import { useEffortCommitments, useArtSources, useLookups } from '../contexts/LookupsContext';
import { ConfirmModal } from '../components/common/ConfirmModal';

const ALL_FILTER = -1;

export function ArtDatabasePage() {
  const [arts, setArts] = useState<Art[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [filterEffort, setFilterEffort] = useState<number>(ALL_FILTER);
  const [filterSource, setFilterSource] = useState<number>(ALL_FILTER);
  const [editingArtId, setEditingArtId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateArtRequest>({
    name: '',
    description: '',
    summary: '',
    minLevel: 1,
    effortCost: 0,
    sourceId: 1,
  });
  const [editForm, setEditForm] = useState<UpdateArtRequest | null>(null);
  const [pendingDelete, setPendingDelete] = useState<{ id: string; name: string } | null>(null);
  const effortOptions = useEffortCommitments();
  const sourceOptions = useArtSources();
  const { effortCommitmentById, artSourceById } = useLookups();

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
    setForm({ name: '', description: '', summary: '', minLevel: 1, effortCost: 0, sourceId: 1 });
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
      sourceId: art.sourceId,
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

  const handleDelete = (id: string, name: string) => {
    setPendingDelete({ id, name });
  };

  const filteredArts = arts.filter(a => {
    if (filterEffort !== ALL_FILTER && a.effortCost !== filterEffort) return false;
    if (filterSource !== ALL_FILTER && a.sourceId !== filterSource) return false;
    return true;
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

      <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap', marginBottom: '1rem' }}>
        <div className="form-group">
          <label>Filter by Effort</label>
          <select value={filterEffort} onChange={e => setFilterEffort(parseInt(e.target.value))}>
            <option value={ALL_FILTER}>Any cost</option>
            {effortOptions.map(o => (
              <option key={o.id} value={o.id}>{o.displayName}</option>
            ))}
          </select>
        </div>
        <div className="form-group">
          <label>Filter by Source</label>
          <select value={filterSource} onChange={e => setFilterSource(parseInt(e.target.value))}>
            <option value={ALL_FILTER}>All sources</option>
            {sourceOptions.map(o => (
              <option key={o.id} value={o.id}>{o.displayName}</option>
            ))}
          </select>
        </div>
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
                        Min Level {art.minLevel} · Effort: {effortCommitmentById.get(art.effortCost)?.displayName ?? '—'} · {artSourceById.get(art.sourceId)?.displayName ?? '—'}
                      </div>
                      {art.summary && (
                        <div style={{ fontSize: '0.875rem', color: 'var(--text-muted)', marginTop: '0.25rem' }}>
                          {art.summary}
                        </div>
                      )}
                    </div>
                    <div style={{ display: 'flex', gap: '0.5rem' }}>
                      <button className="sm" onClick={() => handleEditStart(art)}>Edit</button>
                      <button className="sm danger" onClick={() => handleDelete(art.id, art.name)}>Delete</button>
                    </div>
                  </div>
                  <p style={{ margin: '0.5rem 0 0 0', fontSize: '0.875rem' }}>{art.description}</p>
                </>
              )}
            </div>
          ))}
        </div>
      )}

      <ConfirmModal
        isOpen={pendingDelete !== null}
        title="Delete Art"
        message={<>Delete <strong>{pendingDelete?.name}</strong>? This cannot be undone.</>}
        onConfirm={async () => {
          await artsApi.delete(pendingDelete!.id);
          setPendingDelete(null);
          refreshArts();
        }}
        onCancel={() => setPendingDelete(null)}
      />
    </div>
  );
}
