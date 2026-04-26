export interface FocusEffectTemplate {
  type: string;
  numericValue: number;
  valueType: string;
  condition: string;
  targetSkill?: string;
  targetAttribute?: string;
  description?: string;
}

export interface FocusDefinition {
  id: string;
  name: string;
  description: string | null;
  level1Description: string;
  level2Description: string | null;
  hasLevel2: boolean;
  canTakeMultipleTimes: boolean;
  level1Effects: FocusEffectTemplate[];
  level2Effects: FocusEffectTemplate[];
}

export interface CreateFocusDefinitionRequest {
  name: string;
  description?: string;
  level1Description: string;
  level2Description?: string;
  canTakeMultipleTimes: boolean;
  level1Effects: FocusEffectTemplate[];
  level2Effects: FocusEffectTemplate[];
}

export interface UpdateFocusDefinitionRequest {
  name: string;
  description?: string;
  level1Description: string;
  level2Description?: string;
  canTakeMultipleTimes: boolean;
  level1Effects: FocusEffectTemplate[];
  level2Effects: FocusEffectTemplate[];
}
