import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/auth.models';
import { Pet, CreatePetRequest, UpdatePetRequest } from '../models/feature.models';

@Injectable({ providedIn: 'root' })
export class PetService {
  private readonly BASE = 'http://localhost:5281/api/Pet';

  constructor(private http: HttpClient) {}

  getMyPets(): Observable<ApiResponse<Pet[]>> {
    return this.http.get<ApiResponse<Pet[]>>(`${this.BASE}/myPets`);
  }

  createPet(req: CreatePetRequest): Observable<ApiResponse<Pet>> {
    return this.http.post<ApiResponse<Pet>>(this.BASE, req);
  }

  updatePet(id: number, req: UpdatePetRequest): Observable<ApiResponse<Pet>> {
    return this.http.put<ApiResponse<Pet>>(`${this.BASE}/${id}`, req);
  }

  deletePet(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(`${this.BASE}/${id}`);
  }

  uploadImage(id: number, file: File): Observable<ApiResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.put<ApiResponse<string>>(
      `http://localhost:5281/api/Pets/${id}/image`, formData
    );
  }

  deleteImage(id: number): Observable<ApiResponse> {
    return this.http.delete<ApiResponse>(
      `http://localhost:5281/api/Pets/${id}/image`
    );
  }
}