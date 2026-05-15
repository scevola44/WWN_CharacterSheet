import { useState, useEffect } from 'react';

export function useActiveSection(sectionIds: string[]): string {
  const [activeSection, setActiveSection] = useState(sectionIds[0] ?? '');

  useEffect(() => {
    if (sectionIds.length === 0) return;

    const scrollContainer = document.querySelector('.app-main') as HTMLElement | null;
    if (!scrollContainer) return;

    const update = () => {
      const containerRect = scrollContainer.getBoundingClientRect();
      const triggerY = containerRect.top + containerRect.height * 0.3;

      let current = sectionIds[0];
      for (const id of sectionIds) {
        const el = document.getElementById(id);
        if (el && el.getBoundingClientRect().top <= triggerY) {
          current = id;
        }
      }
      setActiveSection(current);
    };

    scrollContainer.addEventListener('scroll', update, { passive: true });
    update();

    return () => scrollContainer.removeEventListener('scroll', update);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [sectionIds.join(',')]);

  return activeSection;
}
