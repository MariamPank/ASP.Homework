import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { LoginRequest } from '../../../models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  form: LoginRequest = {
    email: '',
    password: '',
  };

  isLoading = false;
  errorMessage = '';
  showPassword = false;

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    if (!this.form.email || !this.form.password) {
      this.errorMessage = 'Please fill in all fields.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authService.login(this.form).subscribe({
     next: (res) => {
      this.isLoading = false;
      if (res.value?.accessToken) {
        this.router.navigate(['/profile']);
      } else {
        this.errorMessage = res.message || 'Login failed.';
      }
      },
    error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Login failed. Please try again.';
    },
    });
  }
}

