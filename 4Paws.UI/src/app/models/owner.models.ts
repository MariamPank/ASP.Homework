// ─── Enums ─────────────────────────────────────────────────────────────────

export enum Rating {
  VeryBad = 1,
  Bad = 2,
  Average = 3,
  Good = 4,
  Excellent = 5,
}

export enum ListingStatus {
  Open = 1,
  Closed = 2,
  Cancelled = 3,
}

export enum AgreementStatus {
  Active = 1,
  Inactive = 2,
  Completed = 3,
}

// ─── Request Models ────────────────────────────────────────────────────────

export interface CreateOwnerProfileRequest {
  userName: string;
  bio?: string;
}

// ─── Response Models ───────────────────────────────────────────────────────

export interface OwnerListingShort {
  id: number;
  title: string;
  status: ListingStatus;
}

export interface OwnerAgreementShort {
  id: number;
  status: AgreementStatus;
  petName: string;
  careGiverName: string;
}

export interface OwnerDashboard {
  ownerId: number;
  userName: string;
  ownerRating: Rating;
  totalPets: number;
  totalListings: number;
  activeListings: number;
  totalAgreements: number;
  activeAgreements: number;
  completedAgreements: number;
  recentListings: OwnerListingShort[];
  recentAgreements: OwnerAgreementShort[];
}

export interface ApiResponse<T = any> {
  status: number;
  message: string | null;
  errors: string | null;
  value?: T;
}