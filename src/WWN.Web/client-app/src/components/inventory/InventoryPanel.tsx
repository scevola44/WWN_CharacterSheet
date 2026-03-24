import { useState } from 'react';
import { SectionCard } from '../layout/SectionCard';
import { characterApi } from '../../api/characterApi';
import type { CharacterDetail, AddItemRequest, ItemInfo } from '../../types/character';

const SLOT_TYPES = ['Stowed', 'Readied', 'Equipped'] as const;

export function InventoryPanel({ character, onUpdate }: {
  character: CharacterDetail;
  onUpdate: (c: CharacterDetail) => void;
}) {
  const [showAdd, setShowAdd] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [itemType, setItemType] = useState('Item');
  const [form, setForm] = useState<AddItemRequest>({
    name: '', encumbrance: 1, itemType: 'Item', quantity: 1, combatSkill: 'Stab',
  });

  const resetForm = () => {
    setForm({ name: '', encumbrance: 1, itemType: 'Item', quantity: 1, combatSkill: 'Stab' });
    setItemType('Item');
  };

  const handleAdd = async () => {
    if (!form.name.trim()) return;
    const req: AddItemRequest = { ...form, itemType };
    const updated = await characterApi.addItem(character.id, req);
    onUpdate(updated);
    setShowAdd(false);
    resetForm();
  };

  const handleEdit = (item: ItemInfo) => {
    setEditingId(item.id);
    setItemType(item.itemType);
    setForm({
      name: item.name,
      description: item.description ?? undefined,
      encumbrance: item.encumbrance,
      quantity: item.quantity,
      itemType: item.itemType,
      damageDieCount: item.itemType === 'Weapon' ? parseInt(item.damageDie?.split('d')[0] || '1') : undefined,
      damageDieSides: item.itemType === 'Weapon' ? parseInt(item.damageDie?.split('d')[1] || '6') : undefined,
      attributeModifier: item.attributeModifier ?? undefined,
      combatSkill: item.combatSkill ?? undefined,
      shockDamage: item.shockDamage ?? undefined,
      shockAcThreshold: item.shockAcThreshold ?? undefined,
      tags: item.tags ?? undefined,
      acBonus: item.acBonus ?? undefined,
      isShield: item.isShield ?? undefined,
    });
  };

  const handleSaveEdit = async () => {
    if (!form.name.trim()) return;
    const req: AddItemRequest = { ...form, itemType };
    const updated = await characterApi.updateItem(character.id, editingId!, req);
    onUpdate(updated);
    setEditingId(null);
    resetForm();
  };

  const handleRemove = async (itemId: string) => {
    await characterApi.removeItem(character.id, itemId);
    const updated = await characterApi.get(character.id);
    onUpdate(updated);
  };

  const handleSlotChange = async (itemId: string, slotType: string) => {
    const updated = await characterApi.changeSlot(character.id, itemId, slotType);
    onUpdate(updated);
  };

  const renderItemForm = (isEditing: boolean) => (
    <div style={{ marginTop: '0.75rem' }}>
      <div className="form-group">
        <label>Item Type</label>
        <select value={itemType} onChange={e => setItemType(e.target.value)} disabled={isEditing}>
          <option value="Item">General Item</option>
          <option value="Weapon">Weapon</option>
          <option value="Armor">Armor</option>
        </select>
      </div>
      <div className="form-row">
        <div className="form-group">
          <label>Name</label>
          <input value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
        </div>
        <div className="form-group">
          <label>Encumbrance</label>
          <input type="number" value={form.encumbrance}
            onChange={e => setForm({ ...form, encumbrance: +e.target.value })} />
        </div>
      </div>

      {itemType === 'Weapon' && (
        <div className="form-row">
          <div className="form-group">
            <label>Damage Dice (count)</label>
            <input type="number" value={form.damageDieCount ?? 1}
              onChange={e => setForm({ ...form, damageDieCount: +e.target.value })} />
          </div>
          <div className="form-group">
            <label>Damage Dice (sides)</label>
            <input type="number" value={form.damageDieSides ?? 6}
              onChange={e => setForm({ ...form, damageDieSides: +e.target.value })} />
          </div>
          <div className="form-group">
            <label>Combat Skill</label>
            <select value={form.combatSkill ?? 'Stab'}
              onChange={e => setForm({ ...form, combatSkill: e.target.value })}>
              <option value="Stab">Stab</option>
              <option value="Shoot">Shoot</option>
              <option value="Punch">Punch</option>
              <option value="Magic">Magic</option>
            </select>
          </div>
          <div className="form-group">
            <label>Attribute</label>
            <select value={form.attributeModifier ?? 'Strength'}
              onChange={e => setForm({ ...form, attributeModifier: e.target.value })}>
              <option>Strength</option>
              <option>Dexterity</option>
              <option>Intelligence</option>
              <option>Wisdom</option>
              <option>Charisma</option>
            </select>
          </div>
          <div className="form-group">
            <label>Tags</label>
            <select value={form.tags ?? 'Melee'}
              onChange={e => setForm({ ...form, tags: e.target.value })}>
              <option value="Melee">Melee</option>
              <option value="Ranged">Ranged</option>
              <option value="Melee, TwoHanded">Melee + Two-Handed</option>
              <option value="Melee, AP">Melee + AP</option>
              <option value="Ranged, AP">Ranged + AP</option>
              <option value="Ranged, Reload">Ranged + Reload</option>
              <option value="Ranged, Thrown">Thrown</option>
              <option value="Melee, Subtle">Melee + Subtle</option>
              <option value="Melee, Long">Melee + Long (Reach)</option>
            </select>
          </div>
          <div className="form-group">
            <label>Shock Dmg</label>
            <input type="number"
              value={form.shockDamage ?? ''}
              onChange={e => setForm({ ...form, shockDamage: e.target.value === '' ? undefined : +e.target.value })} />
          </div>
          <div className="form-group">
            <label>Shock AC ≤</label>
            <input type="number"
              value={form.shockAcThreshold ?? ''}
              onChange={e => setForm({ ...form, shockAcThreshold: e.target.value === '' ? undefined : +e.target.value })} />
          </div>
        </div>
      )}

      {itemType === 'Armor' && (
        <div className="form-row">
          <div className="form-group">
            <label>AC Bonus</label>
            <input type="number" value={form.acBonus ?? 0}
              onChange={e => setForm({ ...form, acBonus: +e.target.value })} />
          </div>
          <div className="form-group">
            <label>Is Shield?</label>
            <select value={form.isShield ? 'true' : 'false'}
              onChange={e => setForm({ ...form, isShield: e.target.value === 'true' })}>
              <option value="false">No</option>
              <option value="true">Yes</option>
            </select>
          </div>
        </div>
      )}

      <div className="modal-actions">
        <button className="secondary" onClick={() => {
          setEditingId(null);
          setShowAdd(false);
          resetForm();
        }}>Cancel</button>
        <button onClick={isEditing ? handleSaveEdit : handleAdd}>
          {isEditing ? 'Save Item' : 'Add Item'}
        </button>
      </div>
    </div>
  );

  return (
    <SectionCard title="Inventory">
      {character.inventory.map(item => (
        editingId === item.id ? (
          <div key={item.id}>{renderItemForm(true)}</div>
        ) : (
          <div key={item.id} className="item-row">
            <div className="item-info">
              <div className="item-name">
                {item.name}
                {item.itemType !== 'Item' && (
                  <span style={{ fontSize: '0.75rem', color: 'var(--primary)', marginLeft: '0.5rem' }}>
                    [{item.itemType}]
                  </span>
                )}
              </div>
              <div className="item-meta">
                Enc: {item.encumbrance} | Qty: {item.quantity}
                {item.itemType === 'Weapon' && ` | ${item.damageDie} dmg`}
                {item.itemType === 'Armor' && ` | AC +${item.acBonus}${item.isShield ? ' (Shield)' : ''}`}
              </div>
            </div>
            <div className="item-actions">
              <select
                value={item.slotType}
                onChange={e => handleSlotChange(item.id, e.target.value)}
                style={{ fontSize: '0.75rem' }}
              >
                {SLOT_TYPES.map(s => <option key={s} value={s}>{s}</option>)}
              </select>
              <button className="sm" onClick={() => handleEdit(item)}>Edit</button>
              <button className="sm danger" onClick={() => handleRemove(item.id)}>X</button>
            </div>
          </div>
        )
      ))}

      {showAdd && !editingId ? (
        renderItemForm(false)
      ) : !editingId ? (
        <button className="sm" style={{ marginTop: '0.5rem' }} onClick={() => setShowAdd(true)}>
          + Add Item
        </button>
      ) : null}
    </SectionCard>
  );
}
