namespace _4Paws.DTOs.Admin.Responses
{
    public class AdminStatsResponse
    {
        public int TotalUsers { get; set; }
        public int TotalOwners { get; set; }
        public int TotalCareGivers { get; set; }
        public int TotalPets { get; set; }
        public int TotalListings { get; set; }
        public int ActiveListings { get; set; }
        public int TotalApplications { get; set; }
        public int TotalAgreements { get; set; }
        public int ActiveAgreements { get; set; }
        public int CompletedAgreements { get; set; }
        public int BannedUsers { get; set; }

        // ── Soft delete stats ─────────────────────────────────────────────
        public int DeletedUsers { get; set; }
        public int DeletedListings { get; set; }
    }
}
