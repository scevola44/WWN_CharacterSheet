import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import type { ReactNode } from 'react';
import type { Lookups, LookupValue } from '../types/lookups';
import { lookupsApi } from '../api/lookupsApi';

interface LookupsContextValue {
  lookups: Lookups;
  effortCommitmentById: Map<number, LookupValue>;
  effortCommitmentByCode: Map<string, LookupValue>;
}

const LookupsContext = createContext<LookupsContextValue | null>(null);

export function LookupsProvider({ children }: { children: ReactNode }) {
  const [lookups, setLookups] = useState<Lookups | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    lookupsApi.getAll()
      .then(setLookups)
      .catch(e => setError(e?.message ?? 'Failed to load lookups'));
  }, []);

  const value = useMemo<LookupsContextValue | null>(() => {
    if (!lookups) return null;
    return {
      lookups,
      effortCommitmentById: new Map(lookups.effortCommitment.map(v => [v.id, v])),
      effortCommitmentByCode: new Map(lookups.effortCommitment.map(v => [v.code, v])),
    };
  }, [lookups]);

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
