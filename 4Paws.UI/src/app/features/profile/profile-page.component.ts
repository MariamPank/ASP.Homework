import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { ProfileService } from '../../services/profile.service';
import { AuthService } from '../../services/auth.service';
import { UserProfile } from '../../models/feature.models';

type ActiveTab = 'profile' | 'owner' | 'caregiver' | 'password';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './profile-page.component.html',
  styleUrl: './profile-page.component.scss',
})
export class ProfilePageComponent implements OnInit {
  private userService    = inject(UserService);
  private profileService = inject(ProfileService);
  private authService    = inject(AuthService);
  private router         = inject(Router);
  private cdr            = inject(ChangeDetectorRef);

  user: UserProfile | null = null;
  activeTab: ActiveTab = 'profile';
  isLoading = true;

  // Edit profile
  editForm = { userName: '', email: '', phoneNumber: '' };
  editLoading = false;
  editSuccess = '';
  editError = '';

  // Change password
  pwForm = { oldPassword: '', newPassword: '', confirmPassword: '' };
  pwLoading = false;
  pwSuccess = '';
  pwError = '';
  showOld = false;
  showNew = false;
  showConfirm = false;

  // Create owner
  ownerForm = { userName: '' };
  ownerLoading = false;
  ownerSuccess = '';
  ownerError = '';

  // Create caregiver
  cgForm = { userName: '', bio: '' };
  cgLoading = false;
  cgSuccess = '';
  cgError = '';

  // Avatar
  avatarLoading = false;
  avatarError = '';

  readonly BASE_URL = 'http://localhost:5281';

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.isLoading = true;
    this.userService.getMe().subscribe({
      next: (res) => {
        this.user = res.value ?? null;
        if (this.user) {
          this.editForm.userName    = this.user.fullName;
          this.editForm.email       = this.user.email;
          this.editForm.phoneNumber = this.user.phoneNumber;
        }
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => { this.isLoading = false; this.cdr.detectChanges(); },
    });
  }

  setTab(tab: ActiveTab) {
    this.activeTab = tab;
    this.editSuccess = this.editError = '';
    this.pwSuccess = this.pwError = '';
    this.ownerSuccess = this.ownerError = '';
    this.cgSuccess = this.cgError = '';
  }

  // ── Edit Profile ───────────────────────────────────────────────────────

  saveProfile() {
    this.editError = ''; this.editSuccess = '';
    this.editLoading = true;
    this.userService.editUser({
      userName: this.editForm.userName || undefined,
      email: this.editForm.email || undefined,
      phoneNumber: this.editForm.phoneNumber || undefined,
    }).subscribe({
      next: () => {
        this.editLoading = false;
        this.editSuccess = 'Profile updated successfully!';
        this.loadProfile();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.editLoading = false;
        this.editError = err.error?.message || 'Update failed.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Change Password ────────────────────────────────────────────────────

  changePassword() {
    this.pwError = ''; this.pwSuccess = '';
    if (this.pwForm.newPassword !== this.pwForm.confirmPassword) {
      this.pwError = 'Passwords do not match.'; return;
    }
    this.pwLoading = true;
    this.userService.changePassword({
      oldPassword: this.pwForm.oldPassword,
      newPassword: this.pwForm.newPassword,
    }).subscribe({
      next: () => {
        this.pwLoading = false;
        this.pwSuccess = 'Password changed successfully!';
        this.pwForm = { oldPassword: '', newPassword: '', confirmPassword: '' };
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.pwLoading = false;
        this.pwError = err.error?.message || 'Change failed.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Create Owner ───────────────────────────────────────────────────────

  createOwner() {
    this.ownerError = ''; this.ownerSuccess = '';
    if (!this.ownerForm.userName) { this.ownerError = 'Username is required.'; return; }
    this.ownerLoading = true;
    this.profileService.createOwner({ userName: this.ownerForm.userName }).subscribe({
      next: () => {
        this.ownerLoading = false;
        this.ownerSuccess = 'Owner profile created! Go to the Owner Dashboard.';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.ownerLoading = false;
        this.ownerError = err.error?.message || 'Creation failed.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Create CareGiver ───────────────────────────────────────────────────

  createCaregiver() {
    this.cgError = ''; this.cgSuccess = '';
    if (!this.cgForm.userName) { this.cgError = 'Username is required.'; return; }
    this.cgLoading = true;
    this.profileService.createCaregiver({
      userName: this.cgForm.userName,
      bio: this.cgForm.bio,
    }).subscribe({
      next: () => {
        this.cgLoading = false;
        this.cgSuccess = 'Caregiver profile created! Go to the Caregiver Dashboard.';
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.cgLoading = false;
        this.cgError = err.error?.message || 'Creation failed.';
        this.cdr.detectChanges();
      },
    });
  }

  // ── Avatar ─────────────────────────────────────────────────────────────

  onAvatarSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    this.avatarError = '';
    this.avatarLoading = true;
    this.userService.uploadAvatar(file).subscribe({
      next: () => {
        this.avatarLoading = false;
        this.loadProfile();
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.avatarLoading = false;
        this.avatarError = err.error?.message || 'Upload failed.';
        this.cdr.detectChanges();
      },
    });
  }

  removeAvatar() {
    this.avatarLoading = true;
    this.userService.deleteAvatar().subscribe({
      next: () => { this.avatarLoading = false; this.loadProfile(); },
      error: () => { this.avatarLoading = false; },
    });
  }

  getAvatarUrl(): string {
    return this.user?.avatarUrl
      ? `${this.BASE_URL}${this.user.avatarUrl}`
      : '';
  }

  logout() { this.authService.logout(); this.router.navigate(['/login']); }
  goBack()  { this.router.navigate(['/profile']); }
}