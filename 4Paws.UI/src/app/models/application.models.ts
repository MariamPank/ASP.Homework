// ── Application models ────────────────────────────────────────────────────

export enum ApplicationStatus {
  Pending   = 1,
  Accepted  = 2,
  Rejected  = 3,
  Withdrawn = 4,
}

export enum AppliedBy {
  Owner      = 1,
  CareGiver  = 2,
}

export interface Application {
  id: number;
  listingId: number;
  applicantId: number;
  applicantName: string;
  message: string;
  proposedFee: number | null;
  status: ApplicationStatus;
  createdAt: string;
}

export interface ApplyRequest {
  listingId: number;
  message: string;
  proposedFee?: number;
}

export interface UpdateApplicationStatusRequest {
  status: ApplicationStatus;
}