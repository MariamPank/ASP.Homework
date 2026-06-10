import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import {
  RegisterRequest,
  LoginRequest,
  VerifyEmailRequest,
  ResetPasswordRequest,
  AuthResponse,
  ApiResponse,
} from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly BASE_URL = 'http://localhost:5281/api/auth';

  constructor(private http: HttpClient) {}

  register(req: RegisterRequest): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.BASE_URL}/register`, req);
  }

  login(req: LoginRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.BASE_URL}/login`, req).pipe(
    tap((res) => {
      if (res.value?.accessToken && res.value.accessToken !== 'Verification') {
        localStorage.setItem('token', res.value.accessToken);
      }
    })
  );
}
  

  verifyEmail(req: VerifyEmailRequest): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE_URL}/verify-email`, req);
  }

  forgotPassword(email: string): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(
      `${this.BASE_URL}/forgot-password/${email}`,
      {}
    );
  }

  resetPassword(req: ResetPasswordRequest): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE_URL}/reset-password`, req);
  }

  logout(): void {
    localStorage.removeItem('token');
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }
}