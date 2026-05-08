import './MobileBookmarks.css';

interface Bookmark {
  label: string;
  sectionId: string;
  icon: React.ReactNode;
}

const BOOKMARKS: Bookmark[] = [
  {
    label: 'Basics',
    sectionId: 'identity-section',
    icon: (
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 3c1.66 0 3 1.34 3 3s-1.34 3-3 3-3-1.34-3-3 1.34-3 3-3zm0 14.2c-2.5 0-4.71-1.28-6-3.22.03-1.99 4-3.08 6-3.08 1.99 0 5.97 1.09 6 3.08-1.29 1.94-3.5 3.22-6 3.22z" />
      </svg>
    ),
  },
  {
    label: 'Combat',
    sectionId: 'combat-section',
    icon: (
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M3.8 6.8l8-4.8 8 4.8v8.8L11.8 22 3.8 15.6zm7.2-2.4l-4 2.4v6l4 4 4-4v-6l-4-2.4z" />
        <path d="M11.8 9l2.8 2.8-2.8 2.8-2.8-2.8z" />
      </svg>
    ),
  },
  {
    label: 'Abilities',
    sectionId: 'abilities-section',
    icon: (
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z" />
      </svg>
    ),
  },
  {
    label: 'Inventory',
    sectionId: 'inventory-section',
    icon: (
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M7 18c-1.1 0-1.99.9-1.99 2S5.9 22 7 22s2-.9 2-2-0.9-2-2-2zM1 2v2h2l3.6 7.59-1.35 2.45c-.16.28-.25.61-.25.96 0 1.1.9 2 2 2h12v-2H7.42c-.14 0-.25-.11-.25-.25l0.02-.12.36-.59h8.54c.77 0 1.44-.47 1.78-1.15l3.22-5.8c.15-.28.24-.61.24-.96 0-1.1-.9-2-2-2H5.21l-.94-2H1zm16 16c-1.1 0-1.99.9-1.99 2s.89 2 1.99 2 2-.9 2-2-0.9-2-2-2z" />
      </svg>
    ),
  },
  {
    label: 'Magic',
    sectionId: 'magic-section',
    icon: (
      <svg viewBox="0 0 24 24" fill="currentColor">
        <path d="M12 2l4 8h7l-5.5 4 2 8-6.5-5-6.5 5 2-8L1 10h7l4-8z" />
      </svg>
    ),
  },
];

export function MobileBookmarks() {
  const handleScroll = (sectionId: string) => {
    const element = document.getElementById(sectionId);
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  };

  return (
    <nav className="mobile-bookmarks">
      {BOOKMARKS.map((bookmark) => (
        <button
          key={bookmark.sectionId}
          className="bookmark-button"
          onClick={() => handleScroll(bookmark.sectionId)}
          title={bookmark.label}
          aria-label={`Jump to ${bookmark.label}`}
        >
          <div className="bookmark-icon">{bookmark.icon}</div>
          <div className="bookmark-label">{bookmark.label}</div>
        </button>
      ))}
    </nav>
  );
}
