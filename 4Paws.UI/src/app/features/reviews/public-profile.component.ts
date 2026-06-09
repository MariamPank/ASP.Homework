import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ReviewService } from '../../services/review.service';
import { AuthService } from '../../services/auth.service';
import { Review, Rating } from '../../models/review.models';
import { ApiResponse } from '../../models/auth.models';

interface PublicOwnerProfile {
  id: number;
  userName: string;
  ownerRating: number;
  userId: number;
}

interface PublicCaregiverProfile {
  id: number;
  userName: string;
  caregiverRating: number;
  userId: number;
}

@Component({
  selector: 'app-public-profile',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './public-profile.component.html',
  styleUrl: './public-profile.component.scss',
})
export class PublicProfileComponent implements OnInit {
  private route         = inject(ActivatedRoute);
  private http          = inject(HttpClient);
  private reviewService = inject(ReviewService);
  private authService   = inject(AuthService);
  private router        = inject(Router);
  private cdr           = inject(ChangeDetectorRef);

  readonly BASE = 'http://localhost:5281/api';

  profileType: 'owner' | 'caregiver' = 'owner';
  profileId: number = 0;

  ownerProfile: PublicOwnerProfile | null = null;
  caregiverProfile: PublicCaregiverProfile | null = null;

  reviews: Review[] = [];
  isLoading = true;
  errorMessage = '';

  Rating = Rating;

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.profileType = params['type'] as 'owner' | 'caregiver';
      this.profileId   = +params['id'];
      this.loadProfile();
    });
  }

  loadProfile() {
    this.isLoading = true;

    if (this.profileType === 'owner') {
      this.http.get<ApiResponse<PublicOwnerProfile>>(
        `${this.BASE}/Owner/Profile/${this.profileId}`
      ).subscribe({
        next: (res) => {
          this.ownerProfile = res.value ?? null;
          this.loadReviews();
        },
        error: () => {
          this.errorMessage = 'Profile not found.';
          this.isLoading = false;
          this.cdr.detectChanges();
        },
      });
    } else {
      this.http.get<ApiResponse<PublicCaregiverProfile>>(
        `${this.BASE}/CareGiver/Profile/${this.profileId}`
      ).subscribe({
        next: (res) => {
          this.caregiverProfile = res.value ?? null;
          this.loadReviews();
        },
        error: () => {
          this.errorMessage = 'Profile not found.';
          this.isLoading = false;
          this.cdr.detectChanges();
        },
      });
    }
  }

  loadReviews() {
    const obs = this.profileType === 'owner'
      ? this.reviewService.getOwnerReviews(this.profileId)
      : this.reviewService.getCaregiverReviews(this.profileId);

    obs.subscribe({
      next: (res) => {
        this.reviews = res.value ?? [];
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      },
    });
  }

  get profileName(): string {
    return this.ownerProfile?.userName
        ?? this.caregiverProfile?.userName
        ?? '—';
  }

  get profileRating(): number {
    return this.ownerProfile?.ownerRating
        ?? this.caregiverProfile?.caregiverRating
        ?? 0;
  }

  getRatingLabel(r: number): string {
    return ['', 'Very Bad', 'Bad', 'Average', 'Good', 'Excellent'][r] ?? '—';
  }

  getStars(r: number): string {
    return '⭐'.repeat(r) + '☆'.repeat(5 - r);
  }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-GB', {
      day: 'numeric', month: 'short', year: 'numeric'
    });
  }

  goBack() { this.router.navigate(['/listings']); }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}