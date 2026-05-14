export interface LookupValue {
  id: number;
  code: string;
  displayName: string;
  description: string | null;
  sortOrder: number;
}

export interface Lookups {
  effortCommitment: LookupValue[];
  artSources: LookupValue[];
  // Phase 1
  skillNames: LookupValue[];
  attributeNames: LookupValue[];
  itemSlotTypes: LookupValue[];
  saveTypes: LookupValue[];
  // Phase 2
  characterClasses: LookupValue[];
  partialClasses: LookupValue[];
  focusEffectTypes: LookupValue[];
  // Phase 3
  weaponTypes: LookupValue[];
  weaponTags: LookupValue[];
}
