// ─── User Request Models ───────────────────────────────────────────────────

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface EditUserRequest {
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
}

// ─── User Response Models ──────────────────────────────────────────────────

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  createdAt: string;
}

export interface ApiResponse<T = any> {
  status: number;
  message: string | null;
  errors: string | null;
  value?: T;
}