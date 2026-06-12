import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { environment } from '../../environments/environment';

export interface AdminUser {
  id: number;
  username: string;
  email: string;
  fullName: string;
  role: number;
  isBanned: boolean;
  isVerified: boolean;
  isDeleted: boolean;
  createdAt: string;
}

export interface AdminStats {
  totalUsers: number;
  totalListings: number;
  totalApplications: number;
  totalAgreements: number;
  activeListings: number;
  activeAgreements: number;
  completedAgreements: number;
  bannedUsers: number;
  totalOwners: number;
  totalCareGivers: number;
  totalPets: number;
}

export interface AdminListing {
  id: number;
  title: string;
  listingType: number;
  status: number;
  proposedBudget: number;
  ownerId: number | null;
  careGiverId: number | null;
  startDate: string;
  endDate: string;
}

export interface AdminApplication {
  id: number;
  listingId: number;
  applicantName: string;
  message: string;
  status: number;
  createdAt: string;
}

export interface AdminAgreement {
  id: number;
  agreedFee: number;
  status: number;
  startDate: string;
  endDate: string;
  ownerId: number;
  careGiverId: number;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly BASE = `${environment.apiUrl}/Admin`;

  constructor(private http: HttpClient) {}

  getStats(): Observable<ApiResponse<AdminStats>> {
    return this.http.get<ApiResponse<AdminStats>>(`${this.BASE}/stats`);
  }

  getUsers(): Observable<ApiResponse<AdminUser[]>> {
    return this.http.get<ApiResponse<AdminUser[]>>(`${this.BASE}/users`);
  }

  banUser(id: number): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE}/users/${id}/ban`, {});
  }

  unbanUser(id: number): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE}/users/${id}/unban`, {});
  }

  deleteUser(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}/users/${id}`);
  }

  getListings(): Observable<ApiResponse<AdminListing[]>> {
    return this.http.get<ApiResponse<AdminListing[]>>(`${this.BASE}/listings`);
  }

  getApplications(): Observable<ApiResponse<AdminApplication[]>> {
    return this.http.get<ApiResponse<AdminApplication[]>>(`${this.BASE}/applications`);
  }

  getAgreements(): Observable<ApiResponse<AdminAgreement[]>> {
    return this.http.get<ApiResponse<AdminAgreement[]>>(`${this.BASE}/agreements`);
  }

  deleteListing(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}/listings/${id}`);
  }
}