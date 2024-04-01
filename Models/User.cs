using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Models
{
    public class User : BaseModel
    {
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(50)]
        public byte[] Password { get; set; }

        public ICollection<Memory> Memories { get; set; }
        public ICollection<Rate> Rates { get; set; }
    }
}
