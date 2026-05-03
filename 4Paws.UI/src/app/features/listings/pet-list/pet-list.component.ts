import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListingService } from '../services/listing.service';

@Component({
  selector: 'app-pet-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pet-list.component.html',
  styleUrl: './pet-list.component.scss'
})
// Ensure 'export' is present and the name matches your import
export class PetListComponent implements OnInit {
  protected listingService = inject(ListingService);

  ngOnInit(): void {
    this.listingService.fetchAllListings();
  }
}
