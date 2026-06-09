import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AgreementService } from '../../services/agreement.service';
import { AuthService } from '../../services/auth.service';
import { Agreement, AgreementStatus } from '../../models/agreement.models';

@Component({
  selector: 'app-my-agreements',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-agreements.component.html',
  styleUrl: './my-agreements.component.scss',
})
export class MyAgreementsComponent implements OnInit {
  private agreementService = inject(AgreementService);
  private authService      = inject(AuthService);
  router           = inject(Router);
  private cdr              = inject(ChangeDetectorRef);

  agreements: Agreement[] = [];
  isLoading = true;
  errorMessage = '';

  completeLoading: number | null = null;
  completeSuccess = '';
  completeError = '';

  AgreementStatus = AgreementStatus;

  ngOnInit() {
    this.loadAgreements();
  }

  loadAgreements() {
    this.agreementService.getMyAgreements().subscribe({
      next: (res) => {
        this.agreements = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load agreements.';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  complete(agreement: Agreement) {
    if (!confirm(`Mark agreement #${agreement.id} as completed?`)) return;

    this.completeLoading = agreement.id;
    this.completeSuccess = '';
    this.completeError   = '';

    this.agreementService.completeAgreement(agreement.id).subscribe({
      next: () => {
        this.completeLoading = null;
        this.completeSuccess = `Agreement #${agreement.id} marked as completed! 🎉`;
        this.loadAgreements();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.completeLoading = null;
        this.completeError = err.error?.message || 'Failed to complete agreement.';
        this.cdr.detectChanges();
      },
    });
  }

  getStatusLabel(s: AgreementStatus): string {
    const map: Record<AgreementStatus, string> = {
      [AgreementStatus.Active]:    'Active',
      [AgreementStatus.Inactive]:  'Inactive',
      [AgreementStatus.Completed]: 'Completed',
    };
    return map[s] ?? 'Unknown';
  }

  getStatusClass(s: AgreementStatus): string {
    const map: Record<AgreementStatus, string> = {
      [AgreementStatus.Active]:    'status-active',
      [AgreementStatus.Inactive]:  'status-inactive',
      [AgreementStatus.Completed]: 'status-completed',
    };
    return map[s] ?? '';
  }

  formatDate(d: string | null): string {
    if (!d) return '—';
    return new Date(d).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric',
    });
  }

  leaveReview(ag: Agreement) {
    this.router.navigate(['/leave-review'], {
      queryParams: {
        agreementId: ag.id,
        careGiverId: ag.careGiverId,
        name: 'CareGiver',
      }
    });
  }

  goBack() { this.router.navigate(['/profile']); }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}