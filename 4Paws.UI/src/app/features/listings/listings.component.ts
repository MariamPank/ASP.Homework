import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ListingService } from '../../services/listing.service';
import { ApplicationService } from '../../services/application.service';
import { AuthService } from '../../services/auth.service';
import { Listing, ListingType, ListingStatus } from '../../models/feature.models';
import { Application, ApplicationStatus } from '../../models/application.models';

@Component({
  selector: 'app-listings',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './listings.component.html',
  styleUrl: './listings.component.scss',
})
export class ListingsComponent implements OnInit {
  private listingService = inject(ListingService);
  private appService     = inject(ApplicationService);
  private authService    = inject(AuthService);
  router = inject(Router);
  private cdr            = inject(ChangeDetectorRef);

  allListings: Listing[] = [];
  filteredListings: Listing[] = [];
  isLoading = true;
  errorMessage = '';

  // Filters
  searchTerm = '';
  selectedType: string = 'all';
  maxBudget: number | null = null;
  sortBy: string = 'newest';

  // Apply modal
  applyListing: Listing | null = null;
  applyForm = { message: '', proposedFee: null as number | null };
  applyLoading = false;
  applyError = '';
  applySuccess = '';

  // View applications modal
  viewListing: Listing | null = null;
  viewApplications: Application[] = [];
  viewLoading = false;
  statusLoading: number | null = null;
  statusSuccess = '';

  ListingType        = ListingType;
  ListingStatus      = ListingStatus;
  ApplicationStatus  = ApplicationStatus;

  get isLoggedIn(): boolean { return this.authService.isLoggedIn(); }

  ngOnInit() {
    this.listingService.getAllListings().subscribe({
      next: (res) => {
        this.allListings = res.value ?? [];
        this.applyFilters();
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

  // ── Filters ────────────────────────────────────────────────────────────

  applyFilters() {
    let result = [...this.allListings];
    if (this.selectedType !== 'all')
      result = result.filter(l => l.listingType === +this.selectedType);
    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      result = result.filter(l =>
        l.title.toLowerCase().includes(term) ||
        l.description.toLowerCase().includes(term) ||
        (l.petName && l.petName.toLowerCase().includes(term))
      );
    }
    if (this.maxBudget !== null && this.maxBudget > 0)
      result = result.filter(l => l.proposedBudget <= this.maxBudget!);
    if (this.sortBy === 'newest')
      result.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    else if (this.sortBy === 'budget-asc')
      result.sort((a, b) => a.proposedBudget - b.proposedBudget);
    else if (this.sortBy === 'budget-desc')
      result.sort((a, b) => b.proposedBudget - a.proposedBudget);
    this.filteredListings = result;
    this.cdr.detectChanges();
  }

  clearFilters() {
    this.searchTerm = ''; this.selectedType = 'all';
    this.maxBudget = null; this.sortBy = 'newest';
    this.applyFilters();
  }

  // ── Apply modal ────────────────────────────────────────────────────────

  openApply(listing: Listing) {
    if (!this.isLoggedIn) { this.router.navigate(['/login']); return; }
    this.applyListing  = listing;
    this.applyForm     = { message: '', proposedFee: null };
    this.applyError    = '';
    this.applySuccess  = '';
  }

  closeApply() { this.applyListing = null; }

  submitApply() {
    if (!this.applyListing) return;
    this.applyError = '';
    if (!this.applyForm.message.trim()) {
      this.applyError = 'Message is required.'; return;
    }
    this.applyLoading = true;
    this.appService.apply({
      listingId:   this.applyListing.id,
      message:     this.applyForm.message,
      proposedFee: this.applyForm.proposedFee ?? undefined,
    }).subscribe({
      next: () => {
        this.applyLoading = false;
        this.applySuccess = 'Application sent successfully! 🎉';
        this.cdr.detectChanges();
        setTimeout(() => this.closeApply(), 2000);
      },
      error: (err) => {
        this.applyLoading = false;
        this.applyError = err.error?.message || 'Failed to apply.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── View applications modal (for listing owners) ───────────────────────

  openViewApplications(listing: Listing) {
    this.viewListing      = listing;
    this.viewApplications = [];
    this.viewLoading      = true;
    this.statusSuccess    = '';
    this.appService.getApplicationsForListing(listing.id).subscribe({
      next: (res) => {
        this.viewApplications = res.value ?? [];
        this.viewLoading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.viewLoading = false; this.cdr.detectChanges(); },
    });
  }

  closeViewApplications() { this.viewListing = null; }

  updateStatus(app: Application, status: ApplicationStatus) {
    this.statusLoading = app.id;
    this.statusSuccess = '';
    this.appService.updateStatus(app.id, { status }).subscribe({
      next: () => {
        this.statusLoading = null;
        this.statusSuccess = status === ApplicationStatus.Accepted
          ? `✅ Application accepted!` : `❌ Application rejected.`;
        // Refresh applications list
        if (this.viewListing)
          this.openViewApplications(this.viewListing);
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.statusLoading = null;
        this.cdr.detectChanges();
      },
    });
  }

  // ── Helpers ────────────────────────────────────────────────────────────

  getTypeClass(t: ListingType): string {
    return t === ListingType.OwnerNeedsCareGiver ? 'type-owner' : 'type-caregiver';
  }

  getStatusLabel(s: ApplicationStatus): string {
    return ['', 'Pending', 'Accepted', 'Rejected', 'Withdrawn'][s] ?? '';
  }

  getStatusClass(s: ApplicationStatus): string {
    return ['', 'status-pending', 'status-accepted', 'status-rejected', 'status-withdrawn'][s] ?? '';
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  }

  getDaysLeft(endDate: string): number {
    return Math.max(0, Math.ceil(
      (new Date(endDate).getTime() - Date.now()) / (1000 * 60 * 60 * 24)
    ));
  }

  goBack()  { this.router.navigate(['/profile']); }
  logout()  { this.authService.logout(); this.router.navigate(['/login']); }
}