import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import {
  CreateOwnerRequest, CreateOwnerResponse,
  CreateCaregiverRequest, CreateCaregiverResponse,
} from '../models/feature.models';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly OWNER_BASE    = 'http://localhost:5281/api/Owner';
  private readonly CG_BASE       = 'http://localhost:5281/api/CareGiver';

  constructor(private http: HttpClient) {}

  createOwner(req: CreateOwnerRequest): Observable<ApiResponse<CreateOwnerResponse>> {
    return this.http.post<ApiResponse<CreateOwnerResponse>>(this.OWNER_BASE, req);
  }

  createCaregiver(req: CreateCaregiverRequest): Observable<ApiResponse<CreateCaregiverResponse>> {
    return this.http.post<ApiResponse<CreateCaregiverResponse>>(this.CG_BASE, req);
  }
}