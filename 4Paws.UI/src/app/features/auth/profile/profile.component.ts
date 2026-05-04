import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../../services/user.service';
import { AuthService } from '../../../services/auth.service';
import { UserProfile } from '../../../models/user.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService);
  private authService = inject(AuthService);
  private router = inject(Router);

  user: UserProfile | null = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit() {
    this.userService.getMyProfile().subscribe({
      next: (res) => {
        this.isLoading = false;
        this.user = res.value ?? null;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to load profile.';
      },
    });
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  getInitials(): string {
    if (!this.user) return '?';
    return `${this.user.firstName?.[0] ?? ''}${this.user.lastName?.[0] ?? ''}`.toUpperCase();
  }
}