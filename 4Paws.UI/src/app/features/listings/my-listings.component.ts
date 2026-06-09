import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ListingService } from '../../services/listing.service';
import { PetService } from '../../services/pet.service';
import { AuthService } from '../../services/auth.service';
import { Listing, ListingType, ListingStatus, Pet } from '../../models/feature.models';

@Component({
  selector: 'app-my-listings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './my-listings.component.html',
  styleUrl: './my-listings.component.scss',
})
export class MyListingsComponent implements OnInit {
  private listingService = inject(ListingService);
  private petService     = inject(PetService);
  private authService    = inject(AuthService);
  private router         = inject(Router);
  private cdr            = inject(ChangeDetectorRef);

  listings: Listing[] = [];
  myPets: Pet[] = [];
  isLoading = true;
  errorMessage = '';

  // Create form
  showCreateForm = false;
  createForm = {
    title: '',
    description: '',
    listingType: ListingType.OwnerNeedsCareGiver,
    startDate: '',
    endDate: '',
    proposedBudget: 0,
    petId: undefined as number | undefined,
    petName: '',
  };
  createLoading = false;
  createError = '';

  // Edit
  editingListing: Listing | null = null;
  editForm = { title: '', description: '', proposedBudget: 0, startDate: '', endDate: '' };
  editLoading = false;
  editError = '';

  // Delete
  deleteLoading: number | null = null;

  ListingType = ListingType;
  ListingStatus = ListingStatus;

  ngOnInit() {
    this.loadListings();
    this.petService.getMyPets().subscribe({
      next: (res) => { this.myPets = res.value ?? []; this.cdr.detectChanges(); },
      error: () => {},
    });
  }

  loadListings() {
    this.listingService.getMyListings().subscribe({
      next: (res) => {
        this.listings = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load listings.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  toggleCreateForm() {
    this.showCreateForm = !this.showCreateForm;
    this.createError = '';
  }

  // ── Create ─────────────────────────────────────────────────────────────

  createListing() {
    this.createError = '';
    const f = this.createForm;
    if (!f.title.trim())       { this.createError = 'Title is required.'; return; }
    if (!f.description.trim()) { this.createError = 'Description is required.'; return; }
    if (!f.startDate)          { this.createError = 'Start date is required.'; return; }
    if (!f.endDate)            { this.createError = 'End date is required.'; return; }
    if (f.proposedBudget <= 0) { this.createError = 'Budget must be greater than 0.'; return; }

    this.createLoading = true;
    this.listingService.createListing({
      title:          f.title,
      description:    f.description,
      listingType:    f.listingType,
      startDate:      f.startDate,
      endDate:        f.endDate,
      proposedBudget: f.proposedBudget,
      petId:          f.petId,
      petName:        f.petName || undefined,
    }).subscribe({
      next: () => {
        this.createLoading = false;
        this.showCreateForm = false;
        this.createForm = {
          title: '', description: '', listingType: ListingType.OwnerNeedsCareGiver,
          startDate: '', endDate: '', proposedBudget: 0, petId: undefined, petName: '',
        };
        this.loadListings();
      },
      error: (err) => {
        this.createLoading = false;
        this.createError = err.error?.message || 'Failed to create listing.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Edit ───────────────────────────────────────────────────────────────

  startEdit(listing: Listing) {
    this.editingListing = listing;
    this.editForm = {
      title:          listing.title,
      description:    listing.description,
      proposedBudget: listing.proposedBudget,
      startDate:      listing.startDate.split('T')[0],
      endDate:        listing.endDate.split('T')[0],
    };
    this.editError = '';
  }

  cancelEdit() { this.editingListing = null; }

  saveListing() {
    if (!this.editingListing) return;
    this.editLoading = true;
    this.listingService.updateListing(this.editingListing.id, this.editForm).subscribe({
      next: () => {
        this.editLoading = false;
        this.editingListing = null;
        this.loadListings();
      },
      error: (err) => {
        this.editLoading = false;
        this.editError = err.error?.message || 'Failed to update listing.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Delete ─────────────────────────────────────────────────────────────

  deleteListing(listing: Listing) {
    if (!confirm(`Delete "${listing.title}"?`)) return;
    this.deleteLoading = listing.id;
    this.listingService.deleteListing(listing.id).subscribe({
      next: () => { this.deleteLoading = null; this.loadListings(); },
      error: () => { this.deleteLoading = null; this.cdr.detectChanges(); },
    });
  }

  // ── Helpers ────────────────────────────────────────────────────────────

  getStatusClass(s: ListingStatus): string {
    return ['', 'status-open', 'status-closed', 'status-cancelled'][s] ?? '';
  }

  getStatusLabel(s: ListingStatus): string {
    return ['', 'Open', 'Closed', 'Cancelled'][s] ?? '';
  }

  getTypeLabel(t: ListingType): string {
    return t === ListingType.OwnerNeedsCareGiver ? 'Needs CareGiver' : 'Offers Service';
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
  goBack() { this.router.navigate(['/owner-dashboard']); }
}