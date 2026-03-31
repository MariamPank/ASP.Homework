using System.Security.Claims;

namespace _4Paws.Helper.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int CurrentUserId()
        {
            var claim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst(ClaimTypes.NameIdentifier);

            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
