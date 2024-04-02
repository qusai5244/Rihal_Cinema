using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Services;
using Microsoft.AspNetCore.Authorization;
using Rihal_Cinema.Services.Interfaces;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Models;
using System.ComponentModel.DataAnnotations;

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
        public async Task<IActionResult> GetMovies([FromQuery] PaginationInputDto input)
        {
            var response = await _movieService.GetMovies(input);

            return Ok(response);
        }

        [HttpPost("rate")]
        public async Task<IActionResult> RateMovie([FromBody] MovieRateDto input)
        {
            var userIdClaim = User.FindFirst("id");
            _ = int.TryParse(userIdClaim.Value, out int userId);

            var response = await _movieService.RateMovie(userId,input);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie([FromRoute] int id)
        {
            var response = await _movieService.GetMovie(id);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> MoviesSearch( [FromQuery] [Required] string searchInput, [FromQuery] PaginationInputDto paginationInput)
        {
            var response = await _movieService.MoviesSearch(searchInput, paginationInput);

            return Ok(response);
        }

        [HttpGet("top5RatedMovies")]
        public async Task<IActionResult> GetMyTopFiveRatedMovies()
        {
            var userIdClaim = User.FindFirst("id");
            _ = int.TryParse(userIdClaim.Value, out int userId);

            var response = await _movieService.GetMyTopFiveRatedMovies(userId);

            return Ok(response);
        }
    }
}
