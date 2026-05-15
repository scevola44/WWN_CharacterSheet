import { useState } from 'react';
import { BrowserRouter, Routes, Route, Link, Navigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSun, faMoon } from '@fortawesome/free-solid-svg-icons';
import { SystemThemeIcon } from './components/icons/SystemThemeIcon';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import { ThemeProvider, useTheme } from './contexts/ThemeContext';
import { LookupsProvider } from './contexts/LookupsContext';
import { ProtectedRoute } from './components/common/ProtectedRoute';
import { Sidebar } from './components/navigation/Sidebar';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import { SpellDatabasePage } from './pages/SpellDatabasePage';
import { ArtDatabasePage } from './pages/ArtDatabasePage';
import { ArtSourceAdminPage } from './pages/ArtSourceAdminPage';
import { AdminPage } from './pages/AdminPage';
import { FocusDatabasePage } from './pages/FocusDatabasePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ConfirmEmailPage } from './pages/ConfirmEmailPage';
import { ForgotPasswordPage } from './pages/ForgotPasswordPage';
import { ResetPasswordPage } from './pages/ResetPasswordPage';
import { ErrorDetailModal } from './components/common/ErrorDetailModal';
import './App.css';

function ThemeToggle() {
  const { theme, cycleTheme } = useTheme();

  const getIconAndLabel = () => {
    switch (theme) {
      case 'light':
        return { icon: <FontAwesomeIcon icon={faSun} />, label: 'Switch to dark mode' };
      case 'dark':
        return { icon: <FontAwesomeIcon icon={faMoon} />, label: 'Switch to system mode' };
      case 'system':
        return { icon: <SystemThemeIcon />, label: 'Switch to light mode' };
    }
  };

  const { icon, label } = getIconAndLabel();

  return (
    <button
      className="theme-toggle"
      onClick={cycleTheme}
      aria-label={label}
      title={label}
    >
      {icon}
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
        <Route path="/confirm-email" element={<ConfirmEmailPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />

        {/* Protected routes */}
        <Route element={<ProtectedRoute />}>
          <Route path="/" element={<CharacterListPage />} />
          <Route path="/new" element={<CharacterCreatePage />} />
          <Route path="/character/:id" element={<CharacterSheetPage />} />
          <Route path="/spells" element={<SpellDatabasePage />} />
          <Route path="/arts" element={<ArtDatabasePage />} />
          <Route path="/admin/art-sources" element={<ArtSourceAdminPage />} />
          <Route path="/admin" element={<AdminPage />} />
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
          <LookupsProvider>
            <ErrorDetailModal />
            <div className="app">
              <AppRoutes />
            </div>
          </LookupsProvider>
        </AuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}

export default App;
