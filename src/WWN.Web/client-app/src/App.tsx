import { useState } from 'react';
import { BrowserRouter, Routes, Route, Link, Navigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSun, faMoon } from '@fortawesome/free-solid-svg-icons';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ThemeProvider, useTheme } from './contexts/ThemeContext';
import { ProtectedRoute } from './components/common/ProtectedRoute';
import { Sidebar } from './components/navigation/Sidebar';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import { SpellDatabasePage } from './pages/SpellDatabasePage';
import { ArtDatabasePage } from './pages/ArtDatabasePage';
import { FocusDatabasePage } from './pages/FocusDatabasePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ErrorDetailModal } from './components/common/ErrorDetailModal';
import './App.css';

function ThemeToggle() {
  const { theme, toggleTheme } = useTheme();
  const isDark = theme === 'dark';
  return (
    <button
      className="theme-toggle"
      onClick={toggleTheme}
      aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
      title={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
    >
      <FontAwesomeIcon icon={isDark ? faSun : faMoon} />
    </button>
  );
}

function Header({ onMenuToggle }: { onMenuToggle: () => void }) {
  return (
    <header className="main-header">
      <button className="menu-toggle" onClick={onMenuToggle} aria-label="Toggle menu">
        <span></span>
        <span></span>
        <span></span>
      </button>
      <Link to="/" style={{ textDecoration: 'none' }}>
        <h1>WWN Character Sheet</h1>
      </Link>
      <ThemeToggle />
    </header>
  );
}

function AppRoutes() {
  const { user } = useAuth();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className="app-layout">
      {user && <Sidebar isOpen={sidebarOpen} onClose={() => setSidebarOpen(false)} />}
      <div className="app-main">
        {user && <Header onMenuToggle={() => setSidebarOpen(!sidebarOpen)} />}
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
          <Route path="/arts" element={<ArtDatabasePage />} />
          <Route path="/foci" element={<FocusDatabasePage />} />
        </Route>

        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
      </div>
    </div>
  );
}

function App() {
  return (
    <BrowserRouter>
      <ThemeProvider>
        <AuthProvider>
          <ErrorDetailModal />
          <div className="app">
            <AppRoutes />
          </div>
        </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}

export default App;
