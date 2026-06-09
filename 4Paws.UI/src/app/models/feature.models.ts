// ── User models ───────────────────────────────────────────────────────────

export interface UserProfile {
  id: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  avatarUrl: string | null;
}

export interface EditUserRequest {
  userName?: string;
  email?: string;
  phoneNumber?: string;
}

export interface ChangePasswordRequest {
  oldPassword: string;
  newPassword: string;
}

// ── Owner models ──────────────────────────────────────────────────────────

export interface CreateOwnerRequest {
  userName: string;
}

export interface CreateOwnerResponse {
  id: number;
  userName: string;
  ownerRating: number;
  userId: number;
}

// ── CareGiver models ──────────────────────────────────────────────────────

export interface CreateCaregiverRequest {
  userName: string;
  bio: string;
}

export interface CreateCaregiverResponse {
  id: number;
  userName: string;
  caregiverRating: number;
  userId: number;
}

// ── Pet models ────────────────────────────────────────────────────────────

export interface Pet {
  id: number;
  petName: string;
  petRating: number;
  description: string;
  imageUrl: string | null;
  ownerId: number;
}

export interface CreatePetRequest {
  petName: string;
  description: string;
}

export interface UpdatePetRequest {
  petName?: string;
  description?: string;
}

// ── Listing models ────────────────────────────────────────────────────────

export enum ListingType {
  OwnerNeedsCareGiver   = 1,
  CareGiverOffersService = 2,
}

export enum ListingStatus {
  Open      = 1,
  Closed    = 2,
  Cancelled = 3,
}

export interface Listing {
  id: number;
  title: string;
  description: string;
  listingType: ListingType;
  status: ListingStatus;
  startDate: string;
  endDate: string;
  proposedBudget: number;
  petName: string | null;
  ownerId: number | null;
  careGiverId: number | null;
  petId: number | null;
  createdAt: string;
}

export interface CreateListingRequest {
  title: string;
  description: string;
  listingType: ListingType;
  startDate: string;
  endDate: string;
  proposedBudget: number;
  petId?: number;
  petName?: string;
}

export interface UpdateListingRequest {
  title?: string;
  description?: string;
  proposedBudget?: number;
  startDate?: string;
  endDate?: string;
  petName?: string;
}

// ── Owner Dashboard models ────────────────────────────────────────────────

export enum Rating {
  VeryBad   = 1,
  Bad       = 2,
  Average   = 3,
  Good      = 4,
  Excellent = 5,
}

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

// ── CareGiver Dashboard models ────────────────────────────────────────────

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

// ── Agreement Status (needed by dashboards) ───────────────────────────────

export enum AgreementStatus {
  Active    = 1,
  Inactive  = 2,
  Completed = 3,
}