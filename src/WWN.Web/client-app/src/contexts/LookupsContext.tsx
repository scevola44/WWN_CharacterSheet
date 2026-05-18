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
  skillNameById: Map<number, LookupValue>;
  skillNameByCode: Map<string, LookupValue>;
  attributeNameById: Map<number, LookupValue>;
  attributeNameByCode: Map<string, LookupValue>;
  itemSlotTypeById: Map<number, LookupValue>;
  itemSlotTypeByCode: Map<string, LookupValue>;
  saveTypeById: Map<number, LookupValue>;
  saveTypeByCode: Map<string, LookupValue>;
  characterClassById: Map<number, LookupValue>;
  characterClassByCode: Map<string, LookupValue>;
  partialClassById: Map<number, LookupValue>;
  partialClassByCode: Map<string, LookupValue>;
  focusEffectTypeById: Map<number, LookupValue>;
  focusEffectTypeByCode: Map<string, LookupValue>;
  weaponTypeById: Map<number, LookupValue>;
  weaponTypeByCode: Map<string, LookupValue>;
  weaponTagById: Map<number, LookupValue>;
  weaponTagByCode: Map<string, LookupValue>;
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
      skillNameById: new Map(lookups.skillNames.map(v => [v.id, v])),
      skillNameByCode: new Map(lookups.skillNames.map(v => [v.code, v])),
      attributeNameById: new Map(lookups.attributeNames.map(v => [v.id, v])),
      attributeNameByCode: new Map(lookups.attributeNames.map(v => [v.code, v])),
      itemSlotTypeById: new Map(lookups.itemSlotTypes.map(v => [v.id, v])),
      itemSlotTypeByCode: new Map(lookups.itemSlotTypes.map(v => [v.code, v])),
      saveTypeById: new Map(lookups.saveTypes.map(v => [v.id, v])),
      saveTypeByCode: new Map(lookups.saveTypes.map(v => [v.code, v])),
      characterClassById: new Map(lookups.characterClasses.map(v => [v.id, v])),
      characterClassByCode: new Map(lookups.characterClasses.map(v => [v.code, v])),
      partialClassById: new Map(lookups.partialClasses.map(v => [v.id, v])),
      partialClassByCode: new Map(lookups.partialClasses.map(v => [v.code, v])),
      focusEffectTypeById: new Map(lookups.focusEffectTypes.map(v => [v.id, v])),
      focusEffectTypeByCode: new Map(lookups.focusEffectTypes.map(v => [v.code, v])),
      weaponTypeById: new Map(lookups.weaponTypes.map(v => [v.id, v])),
      weaponTypeByCode: new Map(lookups.weaponTypes.map(v => [v.code, v])),
      weaponTagById: new Map(lookups.weaponTags.map(v => [v.id, v])),
      weaponTagByCode: new Map(lookups.weaponTags.map(v => [v.code, v])),
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

// eslint-disable-next-line react-refresh/only-export-components
export function useSkillNames(): LookupValue[] {
  return useLookups().lookups.skillNames;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useSkillName(id: number): LookupValue | undefined {
  return useLookups().skillNameById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAttributeNames(): LookupValue[] {
  return useLookups().lookups.attributeNames;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAttributeName(id: number): LookupValue | undefined {
  return useLookups().attributeNameById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useItemSlotTypes(): LookupValue[] {
  return useLookups().lookups.itemSlotTypes;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useItemSlotType(id: number): LookupValue | undefined {
  return useLookups().itemSlotTypeById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useSaveTypes(): LookupValue[] {
  return useLookups().lookups.saveTypes;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useSaveType(id: number): LookupValue | undefined {
  return useLookups().saveTypeById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useCharacterClasses(): LookupValue[] {
  return useLookups().lookups.characterClasses;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useCharacterClass(id: number): LookupValue | undefined {
  return useLookups().characterClassById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function usePartialClasses(): LookupValue[] {
  return useLookups().lookups.partialClasses;
}

// eslint-disable-next-line react-refresh/only-export-components
export function usePartialClass(id: number): LookupValue | undefined {
  return useLookups().partialClassById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useFocusEffectTypes(): LookupValue[] {
  return useLookups().lookups.focusEffectTypes;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useFocusEffectType(id: number): LookupValue | undefined {
  return useLookups().focusEffectTypeById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useWeaponTypes(): LookupValue[] {
  return useLookups().lookups.weaponTypes;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useWeaponType(id: number): LookupValue | undefined {
  return useLookups().weaponTypeById.get(id);
}

// eslint-disable-next-line react-refresh/only-export-components
export function useWeaponTags(): LookupValue[] {
  return useLookups().lookups.weaponTags;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useWeaponTag(id: number): LookupValue | undefined {
  return useLookups().weaponTagById.get(id);
}
