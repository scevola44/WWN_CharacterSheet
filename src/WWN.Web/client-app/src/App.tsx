import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
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
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Link to="/" style={{ textDecoration: 'none' }}>
              <h1>WWN Character Sheet</h1>
            </Link>
            <div style={{ display: 'flex', gap: '1.5rem' }}>
              <Link to="/foci" style={{ color: 'var(--text)', textDecoration: 'none' }}>
                Foci Database
              </Link>
              <Link to="/spells" style={{ color: 'var(--text)', textDecoration: 'none' }}>
                Spell Database
              </Link>
            </div>
          </div>
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
