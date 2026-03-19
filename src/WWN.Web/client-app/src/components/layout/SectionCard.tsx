import type { ReactNode } from 'react';

export function SectionCard({ title, children, className = '' }: {
  title: string;
  children: ReactNode;
  className?: string;
}) {
  return (
    <div className={`section-card ${className}`}>
      <h2>{title}</h2>
      {children}
    </div>
  );
}
