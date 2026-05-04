import { Rating, ListingStatus, AgreementStatus } from './owner.models';

export { Rating, ListingStatus, AgreementStatus };

// ─── Request Models ────────────────────────────────────────────────────────

export interface CreateCaregiverProfileRequest {
  userName: string;
  bio?: string;
}

// ─── Response Models ───────────────────────────────────────────────────────

export interface CaregiverListingShort {
  id: number;
  title: string;
  status: ListingStatus;
}

export interface CaregiverAgreementShort {
  id: number;
  status: AgreementStatus;
  petName: string;
  ownerName: string;
}

export interface CaregiverDashboard {
  caregiverId: number;
  userName: string;
  caregiverRating: Rating;
  totalListings: number;
  activeListings: number;
  totalAgreements: number;
  activeAgreements: number;
  completedAgreements: number;
  recentListings: CaregiverListingShort[];
  recentAgreements: CaregiverAgreementShort[];
}

export interface ApiResponse<T = any> {
  status: number;
  message: string | null;
  errors: string | null;
  value?: T;
}