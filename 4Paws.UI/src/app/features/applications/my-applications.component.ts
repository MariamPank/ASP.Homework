import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { ApplicationService } from '../../services/application.service';
import { AuthService } from '../../services/auth.service';
import { Application, ApplicationStatus } from '../../models/application.models';

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-applications.component.html',
  styleUrl: './my-applications.component.scss',
})
export class MyApplicationsComponent implements OnInit {
  private appService  = inject(ApplicationService);
  private authService = inject(AuthService);
  private router      = inject(Router);
  private cdr         = inject(ChangeDetectorRef);

  applications: Application[] = [];
  isLoading = true;
  errorMessage = '';

  ApplicationStatus = ApplicationStatus;

  ngOnInit() {
    this.appService.getMyApplications().subscribe({
      next: (res) => {
        this.applications = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load applications.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  getStatusLabel(s: ApplicationStatus): string {
    const map: Record<ApplicationStatus, string> = {
      [ApplicationStatus.Pending]:   'Pending',
      [ApplicationStatus.Accepted]:  'Accepted',
      [ApplicationStatus.Rejected]:  'Rejected',
      [ApplicationStatus.Withdrawn]: 'Withdrawn',
    };
    return map[s] ?? 'Unknown';
  }

  getStatusClass(s: ApplicationStatus): string {
    const map: Record<ApplicationStatus, string> = {
      [ApplicationStatus.Pending]:   'status-pending',
      [ApplicationStatus.Accepted]:  'status-accepted',
      [ApplicationStatus.Rejected]:  'status-rejected',
      [ApplicationStatus.Withdrawn]: 'status-withdrawn',
    };
    return map[s] ?? '';
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  }

  goBack()  { this.router.navigate(['/profile']); }
  logout()  { this.authService.logout(); this.router.navigate(['/login']); }
}