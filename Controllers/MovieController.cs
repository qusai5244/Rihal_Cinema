using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery]SearchAndPaginationInputDto input)
        {

            var response = await _movieService.GetMovies(input);

            return Ok(response);
        }
    }
}
