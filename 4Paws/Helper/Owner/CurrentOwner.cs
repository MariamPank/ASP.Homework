using _4Paws.Data;
using _4Paws.Helper.Services;

namespace _4Paws.Helper.Owner
{
    public class CurrentOwner : ICurrentOwner
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;

        public CurrentOwner(DataContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }

        public Models.Owner? GetCurrentOwner()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0)
                return null;

            return _db.Owners.FirstOrDefault(x => x.UserId == userId);
        }
    }
}
