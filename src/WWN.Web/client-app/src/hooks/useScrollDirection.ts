import { useState, useEffect, useRef } from 'react';

export function useScrollDirection() {
  const [scrollingDown, setScrollingDown] = useState(false);
  const lastScrollY = useRef(window.scrollY);

  useEffect(() => {
    const handleScroll = () => {
      const currentY = window.scrollY;
      const delta = currentY - lastScrollY.current;

      if (Math.abs(delta) < 10) return;

      setScrollingDown(currentY > 50 && currentY > lastScrollY.current);
      lastScrollY.current = currentY;
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  return scrollingDown;
}
