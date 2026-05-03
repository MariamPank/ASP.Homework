import { inject, Injectable, signal } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { PetListing } from '../../../shared/models/listing.model';

@Injectable({ providedIn: 'root' })
export class ListingService {
  private api = inject(ApiService);

  // This signal will hold your list of pets for the marketplace
  #listings = signal<PetListing[]>([]);
  listings = this.#listings.asReadonly();

  // Call this to fetch data from your ASP.NET backend
  fetchAllListings() {
    this.api.get<PetListing[]>('listings').subscribe({
      next: (data) => this.#listings.set(data),
      error: (err) => console.error('Failed to load pets', err)
    });
  }
}
