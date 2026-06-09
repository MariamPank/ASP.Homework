import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss',
})
export class ResetPasswordComponent implements OnInit {
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef);

  form = {
    email: '',
    code: '',
    password: '',
    confirmPassword: '',
  };

  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showPassword = false;
  showConfirm = false;

  togglePassword() { this.showPassword = !this.showPassword; }
  toggleConfirm()  { this.showConfirm  = !this.showConfirm;  }

  ngOnInit() {
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

    if (!this.form.email || !this.form.code || !this.form.password) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    if (this.form.password !== this.form.confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }

    if (this.form.password.length < 6) {
      this.errorMessage = 'Password must be at least 6 characters.';
      return;
    }

    this.isLoading = true;

    this.authService.resetPassword({
      email: this.form.email,
      code: this.form.code,
      password: this.form.password,
    }).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.successMessage = 'Password reset! Redirecting to login...';
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