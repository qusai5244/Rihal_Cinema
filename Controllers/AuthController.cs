using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody]UserRegisterInputDto input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _authService.CreateUser(input);

            return Ok(response);
        }



    }
}
