import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-role-select',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './role-select.component.html',
  styleUrl: './role-select.component.scss',
})
export class RoleSelectComponent {
  private authService = inject(AuthService);
  constructor(public router: Router) {}

  get isAdmin(): boolean {
    const token = this.authService.getToken();
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      return role === 'Admin';
    } catch {
      return false;
    }
  }

  goToOwner() {
    this.router.navigate(['/owner-dashboard']);
  }
  goToCaregiver() {
    this.router.navigate(['/caregiver-dashboard']);
  }
  goToListings() {
    this.router.navigate(['/listings']);
  }
  goToProfile() {
    this.router.navigate(['/my-profile']);
  }
  goToAdmin() {
    this.router.navigate(['/admin']);
  }
}
