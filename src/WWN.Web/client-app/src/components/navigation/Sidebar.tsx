import { NavLink } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPeopleGroup, faHandSparkles, faHatWizard, faLeaf, faRightFromBracket } from '@fortawesome/free-solid-svg-icons';
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
      <div className={`sidebar-overlay ${isOpen ? 'visible' : ''}`} onClick={onClose} />
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
                <FontAwesomeIcon icon={faPeopleGroup} className="icon" />
                Characters
              </NavLink>
              <NavLink
                to="/foci"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <FontAwesomeIcon icon={faHandSparkles} className="icon" />
                Foci
              </NavLink>
              <NavLink
                to="/spells"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <FontAwesomeIcon icon={faHatWizard} className="icon" />
                Spells
              </NavLink>
              <NavLink
                to="/arts"
                className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
                onClick={handleNavClick}
              >
                <FontAwesomeIcon icon={faLeaf} className="icon" />
                Arts
              </NavLink>
            </div>
          )}
          {user && (
            <div className="nav-actions">
              <button className="logout-btn" onClick={handleLogout}>
                <FontAwesomeIcon icon={faRightFromBracket} className="icon" />
                Sign Out
              </button>
            </div>
          )}
        </nav>
      </aside>
    </>
  );
}
