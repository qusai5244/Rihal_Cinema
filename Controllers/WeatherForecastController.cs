using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICallRihalApiService _rihalApiService;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICallRihalApiService rihalApiService)
        {
            _logger = logger;
            _rihalApiService = rihalApiService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetMoviesByIds")]
        public async Task<IActionResult> GetMoviesByIds()
        {
            try
            {
                var movieContents = await _rihalApiService.GetMoviesByIdsAsync();
                return Ok(movieContents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch movies by IDs");
                return StatusCode(500, "Failed to fetch movies by IDs");
            }
        }

        [HttpGet("username")] // Endpoint to get the username
        public IActionResult GetUserId()
        {
            // Get the authenticated user's ID from claims
            var id = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(id))
            {
                return NotFound(); // Return 404 if ID is not found
            }

            return Ok(id); // Return the ID as response
        }
    }
}