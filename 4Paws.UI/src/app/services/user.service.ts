import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ChangePasswordRequest,
  EditUserRequest,
  UserProfile,
  ApiResponse,
} from '../models/user.models';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly BASE_URL = 'http://localhost:5000/api/users';

  constructor(private http: HttpClient) {}

  // ─── Get My Profile ──────────────────────────────────────────────────────
  // Requires: Bearer token in Authorization header (handled by AuthInterceptor)

  getMyProfile(): Observable<ApiResponse<UserProfile>> {
    return this.http.get<ApiResponse<UserProfile>>(`${this.BASE_URL}/me`);
  }

  // ─── Change Password ─────────────────────────────────────────────────────

  changePassword(req: ChangePasswordRequest): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(
      `${this.BASE_URL}/change-password`,
      req
    );
  }

  // ─── Edit Profile ────────────────────────────────────────────────────────

  editUser(req: EditUserRequest): Observable<ApiResponse<UserProfile>> {
    return this.http.put<ApiResponse<UserProfile>>(
      `${this.BASE_URL}/edit`,
      req
    );
  }

  // ─── Delete Account ──────────────────────────────────────────────────────

  deleteAccount(): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE_URL}`);
  }
}