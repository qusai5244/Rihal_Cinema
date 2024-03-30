using Microsoft.AspNetCore.Mvc;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Controllers
{
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
    }
}