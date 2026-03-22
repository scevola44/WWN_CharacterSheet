import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import { SpellDatabasePage } from './pages/SpellDatabasePage';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <header>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Link to="/" style={{ textDecoration: 'none' }}>
              <h1>WWN Character Sheet</h1>
            </Link>
            <Link to="/spells" style={{ color: 'var(--text)', textDecoration: 'none' }}>
              Spell Database
            </Link>
          </div>
        </header>
        <Routes>
          <Route path="/" element={<CharacterListPage />} />
          <Route path="/new" element={<CharacterCreatePage />} />
          <Route path="/character/:id" element={<CharacterSheetPage />} />
          <Route path="/spells" element={<SpellDatabasePage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
