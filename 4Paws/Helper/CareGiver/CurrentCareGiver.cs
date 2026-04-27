
using _4Paws.Data;
using _4Paws.Helper.Services;

namespace _4Paws.Helper.CareGiver
{
    public class CurrentCareGiver : ICurrentCareGiver
    {

        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;

        public CurrentCareGiver(DataContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public Models.CareGiver? GetCurrentCareGiver()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0)
                return null;

            return _db.CareGivers.FirstOrDefault(x => x.UserId == userId);
        }
    }
}
