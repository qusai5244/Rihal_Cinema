using System.Drawing;

namespace Rihal_Cinema.Dtos.Photo
{
    public class PhotoListOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Size { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
