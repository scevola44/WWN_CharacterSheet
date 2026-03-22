export interface FocusDefinition {
  id: string;
  name: string;
  description: string | null;
  level1Description: string;
  level2Description: string | null;
  hasLevel2: boolean;
  canTakeMultipleTimes: boolean;
}

export interface CreateFocusDefinitionRequest {
  name: string;
  description?: string;
  level1Description: string;
  level2Description?: string;
  canTakeMultipleTimes: boolean;
}

export interface UpdateFocusDefinitionRequest {
  name: string;
  description?: string;
  level1Description: string;
  level2Description?: string;
  canTakeMultipleTimes: boolean;
}
