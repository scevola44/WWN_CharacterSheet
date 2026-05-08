// EffortCommitment is now a lookup (see LookupsContext / /api/lookups).
// Wire format is the integer id from the EffortCommitment enum:
//   None=0, Scene=1, Day=2, Sustained=3.

export interface Art {
  id: string;
  name: string;
  description: string;
  summary: string | null;
  minLevel: number;
  effortCost: number;
  source: string;
}

export interface KnownArt {
  id: string;
  artId: string;
  art: Art;
}

export interface CreateArtRequest {
  name: string;
  description: string;
  summary?: string;
  minLevel: number;
  effortCost: number;
  source: string;
}

export type UpdateArtRequest = CreateArtRequest;

export interface EffortInfo {
  max: number;
  scene: number;
  day: number;
  sustained: number;
}

export interface CommitEffortRequest {
  commitment: number;
  amount?: number;
}

export interface ReleaseSustainedRequest {
  amount?: number;
}
