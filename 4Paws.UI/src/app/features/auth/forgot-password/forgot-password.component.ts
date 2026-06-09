import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  email = '';
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  codeSent = false;

  onSubmit() {
    this.errorMessage = '';
    this.successMessage = '';

    if (!this.email) {
      this.errorMessage = 'Please enter your email address.';
      return;
    }

    this.isLoading = true;

    this.authService.forgotPassword(this.email).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.codeSent = true;
        this.successMessage = 'Reset code sent! Check your inbox.';
        this.cdr.detectChanges();
        // Redirect to reset-password with email pre-filled
        setTimeout(() => {
          this.router.navigate(['/reset-password'], {
            queryParams: { email: this.email }
          });
        }, 1800);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Something went wrong. Please try again.';
        this.cdr.detectChanges();
      },
    });
  }
}