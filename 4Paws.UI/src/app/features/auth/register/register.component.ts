import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private cdr = inject(ChangeDetectorRef);

  form = {
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
  };

  isLoading = false;
  errorMessage = '';
  showPassword = false;
  showConfirm = false;

  togglePassword() { this.showPassword = !this.showPassword; }
  toggleConfirm()  { this.showConfirm  = !this.showConfirm;  }

  onSubmit() {
    this.errorMessage = '';

    if (!this.form.username || !this.form.email || !this.form.password) {
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

    this.authService.register({
      username: this.form.username,
      email: this.form.email,
      password: this.form.password,
    }).subscribe({
      next: (res) => {
        this.isLoading = false;
        // Redirect to verify-email and pass email as query param
        this.router.navigate(['/verify-email'], {
          queryParams: { email: this.form.email }
        });
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
        this.cdr.detectChanges();
      },
    });
  }
}