import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { Lookups, LookupValue } from '../types/lookups';
import { lookupsApi } from '../api/lookupsApi';

interface LookupsContextValue {
  lookups: Lookups;
  effortCommitmentById: Map<number, LookupValue>;
  effortCommitmentByCode: Map<string, LookupValue>;
  artSourceById: Map<number, LookupValue>;
  artSourceByCode: Map<string, LookupValue>;
  refresh: () => void;
}

const LookupsContext = createContext<LookupsContextValue | null>(null);

export function LookupsProvider({ children }: { children: ReactNode }) {
  const [lookups, setLookups] = useState<Lookups | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [revision, setRevision] = useState(0);

  const refresh = useCallback(() => setRevision(r => r + 1), []);

  useEffect(() => {
    lookupsApi.getAll()
      .then(setLookups)
      .catch(e => setError(e?.message ?? 'Failed to load lookups'));
  }, [revision]);

  const value = useMemo<LookupsContextValue | null>(() => {
    if (!lookups) return null;
    return {
      lookups,
      effortCommitmentById: new Map(lookups.effortCommitment.map(v => [v.id, v])),
      effortCommitmentByCode: new Map(lookups.effortCommitment.map(v => [v.code, v])),
      artSourceById: new Map(lookups.artSources.map(v => [v.id, v])),
      artSourceByCode: new Map(lookups.artSources.map(v => [v.code, v])),
      refresh,
    };
  }, [lookups, refresh]);

  if (error) return <div role="alert">Failed to load lookups: {error}</div>;
  if (!value) return <div>Loading…</div>;

  return <LookupsContext.Provider value={value}>{children}</LookupsContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useLookups(): LookupsContextValue {
  const ctx = useContext(LookupsContext);
  if (!ctx) throw new Error('useLookups must be used inside <LookupsProvider>.');
  return ctx;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useEffortCommitments(): LookupValue[] {
  return useLookups().lookups.effortCommitment;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useEffortCommitment(id: number): LookupValue | undefined {
  return useLookups().effortCommitmentById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useArtSources(): LookupValue[] {
  return useLookups().lookups.artSources;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useArtSource(id: number): LookupValue | undefined {
  return useLookups().artSourceById.get(id);
}
