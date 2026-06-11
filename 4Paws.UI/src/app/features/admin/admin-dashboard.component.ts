import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AdminService, AdminUser, AdminStats, AdminListing, AdminApplication, AdminAgreement } from '../../services/admin.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent implements OnInit {
  private adminService = inject(AdminService);
  private authService  = inject(AuthService);
  private router       = inject(Router);
  private cdr          = inject(ChangeDetectorRef);

  stats: AdminStats | null = null;
  users: AdminUser[] = [];
  listings: AdminListing[] = [];
  applications: AdminApplication[] = [];
  agreements: AdminAgreement[] = [];

  activeTab: 'users' | 'listings' | 'applications' | 'agreements' = 'users';
  isLoading = true;
  errorMessage = '';

  ngOnInit() {
    this.loadStats();
    this.loadUsers();
  }

  loadStats() {
    this.adminService.getStats().subscribe({
      next: (res) => { this.stats = res.value ?? null; this.cdr.detectChanges(); },
      error: () => { this.errorMessage = 'Failed to load stats.'; }
    });
  }

  loadUsers() {
    this.adminService.getUsers().subscribe({
      next: (res) => {
        this.users = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Failed to load users.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  setTab(tab: 'listings' | 'applications' | 'agreements') {
    this.activeTab = tab;
    this.isLoading = true;
    if (tab === 'listings') {
      this.adminService.getListings().subscribe({
        next: (res) => { this.listings = res.value ?? []; this.isLoading = false; this.cdr.detectChanges(); },
        error: () => { this.isLoading = false; }
      });
    } else if (tab === 'applications') {
      this.adminService.getApplications().subscribe({
        next: (res) => { this.applications = res.value ?? []; this.isLoading = false; this.cdr.detectChanges(); },
        error: () => { this.isLoading = false; }
      });
    } else if (tab === 'agreements') {
      this.adminService.getAgreements().subscribe({
        next: (res) => { this.agreements = res.value ?? []; this.isLoading = false; this.cdr.detectChanges(); },
        error: () => { this.isLoading = false; }
      });
    }
  }

  ban(user: AdminUser) {
    this.adminService.banUser(user.id).subscribe({
      next: () => { user.isBanned = true; this.cdr.detectChanges(); },
      error: (err) => alert(err.error?.message || 'Failed to ban user.')
    });
  }

  unban(user: AdminUser) {
    this.adminService.unbanUser(user.id).subscribe({
      next: () => { user.isBanned = false; this.cdr.detectChanges(); },
      error: (err) => alert(err.error?.message || 'Failed to unban user.')
    });
  }

  deleteUser(user: AdminUser) {
    if (!confirm(`Delete user "${user.username}"?`)) return;
    this.adminService.deleteUser(user.id).subscribe({
      next: () => { this.users = this.users.filter(u => u.id !== user.id); this.cdr.detectChanges(); },
      error: (err) => alert(err.error?.message || 'Failed to delete user.')
    });
  }

  deleteListing(l: AdminListing) {
    if (!confirm(`Delete listing "${l.title}"?`)) return;
    this.adminService.deleteListing(l.id).subscribe({
      next: () => { this.listings = this.listings.filter(x => x.id !== l.id); this.cdr.detectChanges(); },
      error: (err) => alert(err.error?.message || 'Failed.')
    });
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' });
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}