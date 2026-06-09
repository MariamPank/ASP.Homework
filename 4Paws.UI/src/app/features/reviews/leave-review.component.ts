import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { ReviewService } from '../../services/review.service';
import { AuthService } from '../../services/auth.service';
import { Rating } from '../../models/review.models';

@Component({
  selector: 'app-leave-review',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './leave-review.component.html',
  styleUrl: './leave-review.component.scss',
})
export class LeaveReviewComponent implements OnInit {
  private reviewService = inject(ReviewService);
  private authService   = inject(AuthService);
  private router        = inject(Router);
  private route         = inject(ActivatedRoute);
  private cdr           = inject(ChangeDetectorRef);

  // Pre-filled from query params
  agreementId: number = 0;
  targetOwnerId?: number;
  targetCareGiverId?: number;
  targetPetId?: number;
  targetName: string = '';

  form = {
    rating: Rating.Good,
    comment: '',
  };

  isLoading = false;
  successMessage = '';
  errorMessage = '';

  Rating = Rating;

  // Star rating UI
  stars = [1, 2, 3, 4, 5] as Rating[];
  hoveredStar: number | null = null;

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.agreementId       = +params['agreementId'] || 0;
      this.targetOwnerId     = params['ownerId']     ? +params['ownerId']     : undefined;
      this.targetCareGiverId = params['careGiverId'] ? +params['careGiverId'] : undefined;
      this.targetPetId       = params['petId']       ? +params['petId']       : undefined;
      this.targetName        = params['name']        || 'this person';
      this.cdr.detectChanges();
    });
  }

  setRating(r: Rating) { this.form.rating = r; }
  hoverStar(r: number) { this.hoveredStar = r; }
  clearHover()         { this.hoveredStar = null; }

  isStarFilled(star: number): boolean {
    return star <= (this.hoveredStar ?? this.form.rating);
  }

  getRatingLabel(r: Rating): string {
    const map: Record<Rating, string> = {
      [Rating.VeryBad]:   '😞 Very Bad',
      [Rating.Bad]:       '😕 Bad',
      [Rating.Average]:   '😐 Average',
      [Rating.Good]:      '😊 Good',
      [Rating.Excellent]: '🤩 Excellent',
    };
    return map[r] ?? '';
  }

  submit() {
    this.errorMessage = '';
    if (!this.agreementId) {
      this.errorMessage = 'Invalid agreement.'; return;
    }
    if (!this.targetOwnerId && !this.targetCareGiverId && !this.targetPetId) {
      this.errorMessage = 'No review target specified.'; return;
    }

    this.isLoading = true;
    this.reviewService.createReview({
      agreementId: this.agreementId,
      rating:      this.form.rating,
      comment:     this.form.comment.trim() || undefined,
      ownerId:     this.targetOwnerId,
      careGiverId: this.targetCareGiverId,
      petId:       this.targetPetId,
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.successMessage = 'Review submitted! Thank you 🐾';
        this.cdr.detectChanges();
        setTimeout(() => this.router.navigate(['/my-agreements']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Failed to submit review.';
        this.cdr.detectChanges();
      },
    });
  }

  goBack() { this.router.navigate(['/my-agreements']); }
  logout() { this.authService.logout(); this.router.navigate(['/login']); }
}