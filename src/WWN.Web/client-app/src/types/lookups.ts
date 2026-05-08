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
}
