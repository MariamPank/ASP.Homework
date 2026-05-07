using _4Paws.Data;
using _4Paws.Enums;
using _4Paws.Helper.Services;
using _4Paws.Models;

namespace _4Paws.Helper.Adm
{
    public class Administrator : IAdministrator
    {
        private readonly DataContext _db;
        private readonly ICurrentUserService _currentUser;

        public Administrator(DataContext db, ICurrentUserService currentUser)
        {
            _db = db;
            _currentUser = currentUser;
        }
        public bool IsAdmin()
        {
            var userId = _currentUser.CurrentUserId();
            if (userId == 0)
                return false;

            var user = _db.Users.FirstOrDefault(x => x.Id == userId);
            return user != null && user.Role == UserRole.Admin;
        }
    }
}
