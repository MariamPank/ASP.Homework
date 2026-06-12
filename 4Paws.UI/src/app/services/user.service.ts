import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { UserProfile, EditUserRequest, ChangePasswordRequest} from '../models/feature.models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UserService {
  private readonly BASE = `${environment.apiUrl}/Users`;

  constructor(private http: HttpClient) {}

  getMe(): Observable<ApiResponse<UserProfile>> {
    return this.http.get<ApiResponse<UserProfile>>(`${this.BASE}/me`);
  }

  editUser(req: EditUserRequest): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE}/edit`, req);
  }

  changePassword(req: ChangePasswordRequest): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE}/change-password`, req);
  }

  uploadAvatar(file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.put<ApiResponse<string>>(`${this.BASE}/avatar`, formData);
  }

  deleteAvatar(): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}/avatar`);
  }

  deleteAccount(): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}`);
  }
}