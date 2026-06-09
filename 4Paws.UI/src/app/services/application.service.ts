import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import {
  Application,
  ApplyRequest,
  UpdateApplicationStatusRequest,
} from '../models/application.models';

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly BASE = 'http://localhost:5281/api/Applications';

  constructor(private http: HttpClient) {}

  apply(req: ApplyRequest): Observable<ApiResponse<Application>> {
    return this.http.post<ApiResponse<Application>>(
      `${this.BASE}/apply`, req
    );
  }

  getApplicationsForListing(listingId: number): Observable<ApiResponse<Application[]>> {
    return this.http.get<ApiResponse<Application[]>>(
      `${this.BASE}/listing/${listingId}`
    );
  }

  getMyApplications(): Observable<ApiResponse<Application[]>> {
    return this.http.get<ApiResponse<Application[]>>(
      `${this.BASE}/my-applications`
    );
  }

  updateStatus(
    applicationId: number,
    req: UpdateApplicationStatusRequest
  ): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(
      `${this.BASE}/${applicationId}/status`, req
    );
  }
}