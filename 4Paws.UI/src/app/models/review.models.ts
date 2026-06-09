// ── Review models ─────────────────────────────────────────────────────────

export enum Rating {
  VeryBad   = 1,
  Bad       = 2,
  Average   = 3,
  Good      = 4,
  Excellent = 5,
}

export interface Review {
  id: number;
  agreementId: number;
  reviewerName: string;
  rating: Rating;
  ratingLabel: string;
  comment: string | null;
  createdAt: string;
  ownerId: number | null;
  careGiverId: number | null;
  petId: number | null;
}

export interface CreateReviewRequest {
  agreementId: number;
  rating: Rating;
  comment?: string;
  ownerId?: number;
  careGiverId?: number;
  petId?: number;
}