using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Dtos.Movie
{
    public class MovieRateDto
    {
        public int MovieId { get; set; }
        [Range(1, 10, ErrorMessage = "The rate must be between 1 and 10.")]
        public int Rate { get; set; }
    }
}
