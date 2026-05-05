export type EffortCommitment = 'Scene' | 'Day' | 'Sustained';

export interface Art {
  id: string;
  name: string;
  description: string;
  summary: string | null;
  minLevel: number;
  effortCost: EffortCommitment | null;
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
  effortCost?: EffortCommitment | null;
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
  commitment: EffortCommitment;
  amount?: number;
}

export interface ReleaseSustainedRequest {
  amount?: number;
}
