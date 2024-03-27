using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Models
{
    public class User : BaseModel
    {
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string Password { get; set; }

    }
}
