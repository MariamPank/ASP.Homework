using _4Paws.Common.Results;
using _4Paws.Data;
using _4Paws.DTOs.Agreement.Responses;
using _4Paws.DTOs.Application.Responses;
using _4Paws.Enums;
using _4Paws.Helper.Owner;
using _4Paws.Models;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace _4Paws.Services.Agreement
{
    public class AgreementService : IAgreementService
    {
        private readonly DataContext _db;
        private readonly ICurrentOwner _currentOwner;

        public AgreementService(DataContext db, ICurrentOwner currentOwner)
        {
            _db = db;
            _currentOwner = currentOwner;
        }
        public Result<AgreementResponse> CreateAgreement(int applicationId)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<AgreementResponse>.NotFound("Owner/Caregiver profile not found");
            var app = _db.Applications.FirstOrDefault(x => x.Id == applicationId && x.Status == ApplicationStatus.Accepted);
            if (app == null) return Result<AgreementResponse>.BadRequest("Application must be accepted to create an agreement.");

            var listing = _db.Listings.Find(app.ListingId);

            var agreement = new Models.Agreement
            {
                ListingId = listing.Id,
                ApplicationId = app.Id,
                PetId = ((listing.ListingType == ListingType.OwnerNeedsCareGiver) ? listing.PetId : app.PetId) ?? 0,
                OwnerId = ((listing.ListingType == ListingType.OwnerNeedsCareGiver) ? listing.OwnerId : app.OwnerId) ?? 0,
                CareGiverId = ((listing.ListingType == ListingType.CareGiverOffersService) ? listing.CareGiverId : app.CareGiverId) ?? 0,
                AgreedFee = ((app.ProposedFee.HasValue) ? app.ProposedFee : listing.ProposedBudget) ?? 0,
                StartDate = listing.StartDate,
                EndDate = listing.EndDate,
                Status = AgreementStatus.Active,
                CreatedAt = DateTime.Now,
            };

            _db.Agreements.Add(agreement);
            _db.SaveChanges();

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        public Result<AgreementResponse> GetAgreementById(int id)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<AgreementResponse>.NotFound("Owner/Caregiver profile not found");

            var agreement = _db.Agreements.FirstOrDefault(a => a.Id == id && 
                            (a.OwnerId == owner.Id || a.CareGiverId == owner.Id));
            if (agreement == null)
                return Result<AgreementResponse>.NotFound("No agreement found with the Id");

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        public Result<IEnumerable<AgreementResponse>> GetMyAgreements()
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<IEnumerable<AgreementResponse>>.NotFound("Owner/Caregiver profile not found");

            var agreements = _db.Agreements.Where(x => (x.OwnerId == owner.Id || x.CareGiverId == owner.Id)).ToList();
            if (agreements == null) return Result<IEnumerable<AgreementResponse>>.NotFound("Agreements not found");

            var agreementList = new List<AgreementResponse>();

            foreach (var agreement in agreements)
            {
                agreementList.Add(MapToResponse(agreement));
            }
            return Result<IEnumerable<AgreementResponse>>.Ok(agreementList);
        }

        public Result<AgreementResponse> CompleteAgreement(int id)
        {
            var owner = _currentOwner.GetCurrentOwner();
            if (owner == null)
                return Result<AgreementResponse>.NotFound("Owner/Caregiver profile not found");

            var agreement = _db.Agreements.FirstOrDefault(x => x.Id == id);
            if (agreement == null) return Result<AgreementResponse>.NotFound("Agreement not found");

            if (agreement.OwnerId != owner.Id)
                return Result<AgreementResponse>.BadRequest("Only the pet owner can confirm completion.");
            if (agreement.Status != AgreementStatus.Active)
                return Result<AgreementResponse>.BadRequest("Only active agreements can be completed.");
            agreement.Status = AgreementStatus.Completed;
            agreement.CompleteAt = DateTime.Now;

            _db.SaveChanges();

            return Result<AgreementResponse>.Ok(MapToResponse(agreement));
        }

        private AgreementResponse MapToResponse(Models.Agreement agreement)
        {
            return new AgreementResponse
            {
                Id = agreement.Id,
                Status = agreement.Status,
                StartDate = agreement.StartDate,
                EndDate = agreement.EndDate,
                AgreedFee = agreement.AgreedFee,
                OwnerId = agreement.OwnerId,
                CareGiverId = agreement.CareGiverId,
                PetId = agreement.PetId,
            };
        }
    }
}
