import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OwnerDashboard } from '../models/feature.models';
import { ApiResponse } from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class OwnerService {
  private readonly BASE_URL = 'http://localhost:5281/api/Owner';

  constructor(private http: HttpClient) {}

  // ─── Get Dashboard ────────────────────────────────────────────────────────

  getDashboard(): Observable<ApiResponse<OwnerDashboard>> {
    return this.http.get<ApiResponse<OwnerDashboard>>(
      `${this.BASE_URL}/Dashboard`
    );
  }

  // ─── Get My Listings ──────────────────────────────────────────────────────

  getMyListings(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.BASE_URL}/MyListings`);
  }

  // ─── Get My Agreements ────────────────────────────────────────────────────

  getMyAgreements(): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.BASE_URL}/MyAgreements`);
  }

  // ─── Get Public Profile ───────────────────────────────────────────────────

  getPublicProfile(ownerId: number): Observable<ApiResponse> {
    return this.http.get<ApiResponse>(`${this.BASE_URL}/Profile/${ownerId}`);
  }
}