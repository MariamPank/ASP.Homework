import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { Agreement } from '../models/agreement.models';

@Injectable({ providedIn: 'root' })
export class AgreementService {
  private readonly BASE = 'http://localhost:5281/api/Agreements';

  constructor(private http: HttpClient) {}

  createAgreement(applicationId: number): Observable<ApiResponse> {
    return this.http.post<ApiResponse>(`${this.BASE}/create/${applicationId}`, {});
  }

  getMyAgreements(): Observable<ApiResponse<Agreement[]>> {
    return this.http.get<ApiResponse<Agreement[]>>(`${this.BASE}/my-agreements`);
  }

  completeAgreement(id: number): Observable<ApiResponse> {
    return this.http.put<ApiResponse>(`${this.BASE}/${id}/complete`, {});
  }
}
