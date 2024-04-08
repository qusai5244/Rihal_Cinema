using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Infrastructure.ServiceContext;

namespace Rihal_Cinema.Controllers
{
    public class BaseController : ControllerBase
    {
        public BaseController() { }

        [NonAction]
        public RequestHeaderContent BindRequestHeader()
        {
            var userIdClaim = User.FindFirst("id");
            _ = int.TryParse(userIdClaim.Value, out int userId);
            return new()
            {
                UserId = userId
            };
        }
    }
}
