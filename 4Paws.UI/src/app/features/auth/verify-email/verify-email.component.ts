import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './verify-email.component.html',
  styleUrl: './verify-email.component.scss',
})
export class VerifyEmailComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);

  form = {
    email: '',
    code: '',
  };

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit() {
    // Pre-fill email if passed from register page
    this.route.queryParams.subscribe(params => {
      if (params['email']) {
        this.form.email = params['email'];
        this.cdr.detectChanges();
      }
    });
  }

  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.form.email || !this.form.code) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    this.isLoading = true;

    this.authService.verifyEmail({
      email: this.form.email,
      code: this.form.code,
    }).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = 'Email verified! Redirecting to login...';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/login']), 1800);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Invalid code. Please try again.';
        this.cdr.detectChanges();
      },
    });
  }
}