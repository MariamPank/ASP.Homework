// ── Agreement models ──────────────────────────────────────────────────────

export enum AgreementStatus {
  Active    = 1,
  Inactive  = 2,
  Completed = 3,
}

export interface Agreement {
  id: number;
  status: AgreementStatus;
  startDate: string;
  endDate: string;
  agreedFee: number;
  ownerId: number;
  careGiverId: number;
  petId: number;
  completeAt: string | null;
}