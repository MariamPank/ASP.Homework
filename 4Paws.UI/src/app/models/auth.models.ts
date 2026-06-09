// ─── Auth Request Models ───────────────────────────────────────────────────

export interface RegisterRequest {
  username: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface VerifyEmailRequest {
  email: string;
  code: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  password: string;
}

// ─── Auth Response Models ──────────────────────────────────────────────────

export interface AuthResponse {
  status: number;
  message: string | null;
  errors: string | null;
  value?: {
    accessToken: string;
  };
}

export interface ApiResponse<T = any> {
  status: number;
  message: string | null;
  errors: string | null;
  value?: T;
}