import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { OwnerService } from '../../../services/owner.service';
import { AuthService } from '../../../services/auth.service';
import { OwnerDashboard, Rating, ListingStatus, AgreementStatus } from '../../../models/feature.models';

@Component({
  selector: 'app-owner-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './owner-dashboard.component.html',
  styleUrl: './owner-dashboard.component.scss',
})
export class OwnerDashboardComponent implements OnInit {
  private ownerService = inject(OwnerService);
  private authService = inject(AuthService);
  router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  dashboard: OwnerDashboard | null = null;
  isLoading = true;
  errorMessage = '';

  ListingStatus = ListingStatus;
  AgreementStatus = AgreementStatus;

  ngOnInit() {
    this.ownerService.getDashboard().subscribe({
      next: (res) => {
        this.dashboard = res.value ?? null;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load dashboard.';
        this.cdr.detectChanges();
      },
    });
  }

  getRatingLabel(rating: number): string {
    const labels: { [key: number]: string } = {
      0: 'No rating yet',
      1: 'Very Bad', 2: 'Bad', 3: 'Average', 4: 'Good', 5: 'Excellent',
    };
    return labels[rating] ?? 'Unknown';
  }

  getListingStatusLabel(status: ListingStatus): string {
    return ListingStatus[status] ?? 'Unknown';
  }

  getAgreementStatusLabel(status: AgreementStatus): string {
    return AgreementStatus[status] ?? 'Unknown';
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  goBack() {
    this.router.navigate(['/profile']);
  }
}