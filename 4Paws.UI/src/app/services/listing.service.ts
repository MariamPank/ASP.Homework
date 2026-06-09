import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { Listing, CreateListingRequest, UpdateListingRequest } from '../models/feature.models';

@Injectable({ providedIn: 'root' })
export class ListingService {
  private readonly BASE = 'http://localhost:5281/api/Listings';

  constructor(private http: HttpClient) {}

  getAllListings(): Observable<ApiResponse<Listing[]>> {
    return this.http.get<ApiResponse<Listing[]>>(this.BASE);
  }

  getMyListings(): Observable<ApiResponse<Listing[]>> {
    return this.http.get<ApiResponse<Listing[]>>(`${this.BASE}/my-listings`);
  }

  getById(id: number): Observable<ApiResponse<Listing>> {
    return this.http.get<ApiResponse<Listing>>(`${this.BASE}/${id}`);
  }

  createListing(req: CreateListingRequest): Observable<ApiResponse<Listing>> {
    return this.http.post<ApiResponse<Listing>>(this.BASE, req);
  }

  updateListing(id: number, req: UpdateListingRequest): Observable<ApiResponse<Listing>> {
    return this.http.put<ApiResponse<Listing>>(`${this.BASE}/${id}`, req);
  }

  deleteListing(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}/${id}`);
  }
}