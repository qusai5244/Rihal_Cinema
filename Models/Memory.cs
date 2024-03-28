using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Models
{
    public class Memory : BaseModel
    {
        public int UserId { get; set; }
        public User User { get; set; }        
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        [MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(5000)]
        public string Story { get; set; }
        public ICollection<Photo> Photos { get; set; }

    }
}
