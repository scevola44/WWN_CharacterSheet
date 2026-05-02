import { BrowserRouter, Routes, Route, Link, NavLink, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ProtectedRoute } from './components/common/ProtectedRoute';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import { SpellDatabasePage } from './pages/SpellDatabasePage';
import { FocusDatabasePage } from './pages/FocusDatabasePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ErrorDetailModal } from './components/common/ErrorDetailModal';
import './App.css';

function NavBar() {
  const { user, logout } = useAuth();
  return (
    <header>
      <Link to="/" style={{ textDecoration: 'none' }}>
        <h1>WWN Character Sheet</h1>
      </Link>
      <nav className="tabs">
        {user && (
          <>
            <NavLink to="/" end className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Characters
            </NavLink>
            <NavLink to="/foci" className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Foci
            </NavLink>
            <NavLink to="/spells" className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Spells
            </NavLink>
          </>
        )}
        {user ? (
          <button
            className="tab"
            onClick={logout}
            style={{ background: 'none', border: 'none', cursor: 'pointer' }}
          >
            Sign Out
          </button>
        ) : (
          <NavLink to="/login" className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
            Sign In
          </NavLink>
        )}
      </nav>
    </header>
  );
}

function AppRoutes() {
  return (
    <>
      <NavBar />
      <Routes>
        {/* Public routes */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Protected routes */}
        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<CharacterListPage />} />
          <Route path="/new" element={<CharacterCreatePage />} />
          <Route path="/character/:id" element={<CharacterSheetPage />} />
          <Route path="/spells" element={<SpellDatabasePage />} />
          <Route path="/foci" element={<FocusDatabasePage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </>
  );
}

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <ErrorDetailModal />
        <div className="app">
          <AppRoutes />
        </div>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
