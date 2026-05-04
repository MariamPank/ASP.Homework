import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { CaregiverService } from '../../../services/caregiver.service';
import { AuthService } from '../../../services/auth.service';
import { CaregiverDashboard, ListingStatus, AgreementStatus } from '../../../models/caregiver.models';

@Component({
  selector: 'app-caregiver-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './caregiver-dashboard.component.html',
  styleUrl: './caregiver-dashboard.component.scss',
})
export class CaregiverDashboardComponent implements OnInit {
  private caregiverService = inject(CaregiverService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  dashboard: CaregiverDashboard | null = null;
  isLoading = true;
  errorMessage = '';

  ListingStatus = ListingStatus;
  AgreementStatus = AgreementStatus;

  ngOnInit() {
    this.caregiverService.getDashboard().subscribe({
      next: (res) => {
        this.dashboard = res.value ?? null;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.status === 401
          ? 'Unauthorized. Please log in again.'
          : 'Failed to load dashboard.';
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