import { useState, useEffect, useCallback } from 'react';
import { characterApi } from '../api/characterApi';
import type { CharacterDetail } from '../types/character';

export function useCharacter(id: string | undefined) {
  const [character, setCharacter] = useState<CharacterDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const refresh = useCallback(async () => {
    if (!id) return;
    try {
      setLoading(true);
      const data = await characterApi.get(id);
      setCharacter(data);
      setError(null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Failed to load character');
    } finally {
      setLoading(false);
    }
  }, [id]);

  useEffect(() => {
    refresh();
  }, [refresh]);

  return { character, setCharacter, loading, error, refresh };
}
