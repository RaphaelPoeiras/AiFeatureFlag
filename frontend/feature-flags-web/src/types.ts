export type AuthResponse = {
  userId: string;
  email: string;
  displayName: string;
  accessToken: string;
};

export type FeatureFlagResponse = {
  id: string;
  ownerUserId: string;
  key: string;
  description: string;
  isEnabled: boolean;
  environment: string;
  aiIntegrationHintsJson: string;
  createdAtUtc: string;
  updatedAtUtc: string;
};

export type PublicFeatureFlagSummary = {
  key: string;
  isEnabled: boolean;
  environment: string;
};

/** Mirrors backend `ApiProblemResponse` (`application/problem+json`, camelCase). */
export type ApiProblemResponse = {
  title: string;
  detail: string;
  status: number;
  errorCode: ApiErrorCode;
  traceId?: string;
};

/** Mirrors backend `ApiErrorCode` constants. */
export type ApiErrorCode =
  | "VALIDATION_FAILED"
  | "INVALID_REQUEST_BODY"
  | "CONFLICT"
  | "NOT_FOUND"
  | "INVALID_CREDENTIALS"
  | "MISSING_IDENTITY"
  | "INTERNAL_ERROR";
