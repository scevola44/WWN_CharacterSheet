import { BrowserRouter, Routes, Route, Link } from 'react-router-dom';
import { CharacterListPage } from './pages/CharacterListPage';
import { CharacterCreatePage } from './pages/CharacterCreatePage';
import { CharacterSheetPage } from './pages/CharacterSheetPage';
import './App.css';

function App() {
  return (
    <BrowserRouter>
      <div className="app">
        <header>
          <Link to="/" style={{ textDecoration: 'none' }}>
            <h1>WWN Character Sheet</h1>
          </Link>
        </header>
        <Routes>
          <Route path="/" element={<CharacterListPage />} />
          <Route path="/new" element={<CharacterCreatePage />} />
          <Route path="/character/:id" element={<CharacterSheetPage />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
