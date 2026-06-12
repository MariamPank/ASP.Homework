import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { Review, CreateReviewRequest } from '../models/review.models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private readonly BASE = `${environment.apiUrl}/Reviews`;

  constructor(private http: HttpClient) {}

  createReview(req: CreateReviewRequest): Observable<ApiResponse<Review>> {
    return this.http.post<ApiResponse<Review>>(this.BASE, req);
  }

  getOwnerReviews(ownerId: number): Observable<ApiResponse<Review[]>> {
    return this.http.get<ApiResponse<Review[]>>(
      `${this.BASE}/owner/${ownerId}`
    );
  }

  getCaregiverReviews(careGiverId: number): Observable<ApiResponse<Review[]>> {
    return this.http.get<ApiResponse<Review[]>>(
      `${this.BASE}/caregiver/${careGiverId}`
    );
  }

  getPetReviews(petId: number): Observable<ApiResponse<Review[]>> {
    return this.http.get<ApiResponse<Review[]>>(
      `${this.BASE}/pet/${petId}`
    );
  }
}