import { useState, useEffect, useRef } from 'react';

export function useScrollDirection() {
  const [scrollingDown, setScrollingDown] = useState(false);
  const lastScrollY = useRef(0);

  useEffect(() => {
    const scrollContainer = document.querySelector('.app-main') as HTMLElement | null;
    if (!scrollContainer) return;

    lastScrollY.current = scrollContainer.scrollTop;

    const handleScroll = () => {
      const currentY = scrollContainer.scrollTop;
      const delta = currentY - lastScrollY.current;

      if (Math.abs(delta) < 10) return;

      setScrollingDown(currentY > 50 && currentY > lastScrollY.current);
      lastScrollY.current = currentY;
    };

    scrollContainer.addEventListener('scroll', handleScroll, { passive: true });
    return () => scrollContainer.removeEventListener('scroll', handleScroll);
  }, []);

  return scrollingDown;
}
