// ─── Auth Request Models ───────────────────────────────────────────────────

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface VerifyEmailRequest {
  email: string;
  token: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

// ─── Auth Response Models ──────────────────────────────────────────────────

export interface AuthResponse {
  status: number;
  message: string;
  data?: {
    token: string;
    expiresAt: string;
  };
}

export interface ApiResponse<T = any> {
  status: number;
  message: string;
  data?: T;
}