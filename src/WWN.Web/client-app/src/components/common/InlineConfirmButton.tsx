import { useState, useEffect, useRef } from 'react';

interface InlineConfirmButtonProps {
  onConfirm: () => void;
  label?: string;
  confirmLabel?: string;
  className?: string;
  resetDelayMs?: number;
}

export function InlineConfirmButton({
  onConfirm,
  label = 'Delete',
  confirmLabel = 'Confirm?',
  className,
  resetDelayMs = 4000,
}: InlineConfirmButtonProps) {
  const [confirming, setConfirming] = useState(false);
  const groupRef = useRef<HTMLSpanElement>(null);

  useEffect(() => {
    if (!confirming) return;
    const timer = setTimeout(() => setConfirming(false), resetDelayMs);
    return () => clearTimeout(timer);
  }, [confirming, resetDelayMs]);

  const handleConfirm = () => {
    setConfirming(false);
    onConfirm();
  };

  const handleBlur = (e: React.FocusEvent<HTMLSpanElement>) => {
    if (!groupRef.current?.contains(e.relatedTarget as Node)) {
      setConfirming(false);
    }
  };

  if (!confirming) {
    return (
      <button className={className} onClick={() => setConfirming(true)}>
        {label}
      </button>
    );
  }

  return (
    <span className="inline-confirm-group" ref={groupRef} onBlur={handleBlur}>
      <button className="sm danger" autoFocus onMouseDown={(e) => e.preventDefault()} onClick={handleConfirm}>{confirmLabel}</button>
      <button className="sm secondary" onMouseDown={(e) => e.preventDefault()} onClick={() => setConfirming(false)}>Cancel</button>
    </span>
  );
}
