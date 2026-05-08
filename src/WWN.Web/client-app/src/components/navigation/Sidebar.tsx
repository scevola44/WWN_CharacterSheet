import { NavLink } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import './Sidebar.css';

interface SidebarProps {
  isOpen: boolean;
  onClose: () => void;
}

export function Sidebar({ isOpen, onClose }: SidebarProps) {
  const { user, logout } = useAuth();

  const handleNavClick = () => {
    onClose();
  };

  const handleLogout = () => {
    logout();
    onClose();
  };

  return (
    <>
      {isOpen && <div className="sidebar-overlay" onClick={onClose} />}
      <aside className={`sidebar ${isOpen ? 'open' : ''}`}>
        <nav className="sidebar-nav">
          {user && (
            <div className="nav-links">
              <NavLink
                to="/"
                end
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <span className="icon">👥</span>
                Characters
              </NavLink>
              <NavLink
                to="/foci"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <span className="icon">⚡</span>
                Foci
              </NavLink>
              <NavLink
                to="/spells"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <span className="icon">✨</span>
                Spells
              </NavLink>
              <NavLink
                to="/arts"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <span className="icon">🎨</span>
                Arts
              </NavLink>
            </div>
          )}
          {user && (
            <div className="nav-actions">
              <button className="logout-btn" onClick={handleLogout}>
                <span className="icon">🚪</span>
                Sign Out
              </button>
            </div>
          )}
        </nav>
      </aside>
    </>
  );
}
