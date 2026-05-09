import { useEffect } from 'react';

interface ConfirmModalProps {
  isOpen: boolean;
  title: string;
  message: string | React.ReactNode;
  confirmLabel?: string;
  onConfirm: () => void;
  onCancel: () => void;
}

export function ConfirmModal({
  isOpen,
  title,
  message,
  confirmLabel = 'Delete',
  onConfirm,
  onCancel,
}: ConfirmModalProps) {
  useEffect(() => {
    if (!isOpen) return;
    const onKey = (e: KeyboardEvent) => { if (e.key === 'Escape') onCancel(); };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [isOpen, onCancel]);

  if (!isOpen) return null;

  return (
    <div
      className="modal-overlay"
      style={{ zIndex: 1000 }}
      onClick={onCancel}
    >
      <div className="modal" onClick={e => e.stopPropagation()}>
        <h3>{title}</h3>
        <p style={{ margin: '0 0 0' }}>{message}</p>
        <div className="modal-actions">
          <button className="secondary" onClick={onCancel}>Cancel</button>
          <button className="danger" autoFocus onClick={onConfirm}>{confirmLabel}</button>
        </div>
      </div>
    </div>
  );
}
