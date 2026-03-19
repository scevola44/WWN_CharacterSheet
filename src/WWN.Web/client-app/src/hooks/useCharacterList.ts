import { useState, useEffect, useCallback } from 'react';
import { characterApi } from '../api/characterApi';
import type { CharacterSummary } from '../types/character';

export function useCharacterList() {
  const [characters, setCharacters] = useState<CharacterSummary[]>([]);
  const [loading, setLoading] = useState(true);

  const refresh = useCallback(async () => {
    try {
      setLoading(true);
      const data = await characterApi.list();
      setCharacters(data);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return { characters, loading, refresh };
}
