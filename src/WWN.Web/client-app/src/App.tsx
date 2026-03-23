import { BrowserRouter, Routes, Route, Link, NavLink } from 'react-router-dom';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import { SpellDatabasePage } from './pages/SpellDatabasePage';
import { FocusDatabasePage } from './pages/FocusDatabasePage';
import { ErrorDetailModal } from './components/common/ErrorDetailModal';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <ErrorDetailModal />
      <div className="app">
        <header>
          <Link to="/" style={{ textDecoration: 'none' }}>
            <h1>WWN Character Sheet</h1>
          </Link>
          <nav className="tabs">
            <NavLink to="/" end className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Characters
            </NavLink>
            <NavLink to="/foci" className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Foci
            </NavLink>
            <NavLink to="/spells" className={({ isActive }) => isActive ? 'tab active' : 'tab'}>
              Spells
            </NavLink>
          </nav>
        </header>
        <Routes>
          <Route path="/" element={<CharacterListPage />} />
          <Route path="/new" element={<CharacterCreatePage />} />
          <Route path="/character/:id" element={<CharacterSheetPage />} />
          <Route path="/spells" element={<SpellDatabasePage />} />
          <Route path="/foci" element={<FocusDatabasePage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
