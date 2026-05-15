import './MobileBookmarks.css';
import { useMemo } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import type { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import {
  faUser,
  faChessKnight,
  faStar,
  faPersonHiking,
  faWandSparkles,
} from '@fortawesome/free-solid-svg-icons';
import { useScrollDirection } from '../../hooks/useScrollDirection';
import { useActiveSection } from '../../hooks/useActiveSection';

interface Bookmark {
  label: string;
  sectionId: string;
  icon: IconDefinition;
}

const BOOKMARKS: Bookmark[] = [
  {
    label: 'Basics',
    sectionId: 'identity-section',
    icon: faUser,
  },
  {
    label: 'Combat',
    sectionId: 'combat-section',
    icon: faChessKnight,
  },
  {
    label: 'Abilities',
    sectionId: 'abilities-section',
    icon: faStar,
  },
  {
    label: 'Inventory',
    sectionId: 'inventory-section',
    icon: faPersonHiking,
  },
  {
    label: 'Magic',
    sectionId: 'magic-section',
    icon: faWandSparkles,
  },
];

interface Props {
  hasMagic: boolean;
}

export function MobileBookmarks({ hasMagic }: Props) {
  const scrollingDown = useScrollDirection();

  const visibleBookmarks = hasMagic
    ? BOOKMARKS
    : BOOKMARKS.filter(b => b.sectionId !== 'magic-section');

  const sectionIds = useMemo(
    () => visibleBookmarks.map(b => b.sectionId),
    [visibleBookmarks]
  );
  const activeSection = useActiveSection(sectionIds);

  const handleScroll = (sectionId: string) => {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  };

  return (
    <nav className={`mobile-bookmarks${scrollingDown ? ' mobile-bookmarks--hidden' : ''}`}>
      {visibleBookmarks.map((bookmark) => (
        <button
          key={bookmark.sectionId}
          className={`bookmark-button${activeSection === bookmark.sectionId ? ' bookmark-button--active' : ''}`}
          onClick={() => handleScroll(bookmark.sectionId)}
          title={bookmark.label}
          aria-label={`Jump to ${bookmark.label}`}
        >
          <FontAwesomeIcon icon={bookmark.icon} className="bookmark-icon" />
          <div className="bookmark-label">{bookmark.label}</div>
        </button>
      ))}
    </nav>
  );
}
