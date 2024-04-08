using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Rihal_Cinema.Services.Interfaces;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using System.ComponentModel.DataAnnotations;
using Rihal_Cinema.Dtos.StarSystem;

namespace Rihal_Cinema.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]

    public class MovieController : BaseController
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
            _movieService.Header = BindRequestHeader();
            var response = await _movieService.RateMovie(input);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMovie([FromRoute] int id)
        {
            _movieService.Header = BindRequestHeader();
            var response = await _movieService.GetMovie(id);

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> MoviesSearch([Required] string searchInput, [FromQuery] PaginationInputDto paginationInput)
        {
            var response = await _movieService.MoviesSearch(searchInput, paginationInput);

            return Ok(response);
        }

        [HttpGet("top5RatedMovies")]
        public async Task<IActionResult> GetMyTopFiveRatedMovies()
        {
            _movieService.Header = BindRequestHeader();
            var response = await _movieService.GetMyTopFiveRatedMovies();

            return Ok(response);
        }

        [HttpGet("guessMovie")]
        public async Task<IActionResult> GuessTheMovie([Required] string scrambledName)
        {
            var response = await _movieService.GuessTheMovie(scrambledName);

            return Ok(response);
        }

        [HttpGet("RatingsCompare")]
        public async Task<IActionResult> RatingsCompare()
        {
            _movieService.Header = BindRequestHeader();
            var response = await _movieService.RatingsCompare();

            return Ok(response);
        }
        [HttpPost("StarSystem")]
        public async Task<IActionResult> StarSystem([FromBody] List<int> movieIds)
        {
            var response = await _movieService.StarSystem(movieIds);

            return Ok(response);
        }

    }
}
