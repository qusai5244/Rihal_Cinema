using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Models
{
    public class Rate : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Range(1, 10)]
        public int Value { get; set; }
    }
}
