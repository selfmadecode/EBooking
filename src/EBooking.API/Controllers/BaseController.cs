using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EBooking.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult ReturnResponse(dynamic result)
        {
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        protected Guid UserId
        {
            get { return Guid.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value); }
        }
    }
}
