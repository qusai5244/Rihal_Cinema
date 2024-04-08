using Rihal_Cinema.Dtos.Memory;
using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Dtos.Photo
{
    public class PhotoUploadInputDto : AddPhotoToMemory
    {
        [Required]
        public int MemoryId { get; set; }
    }
}
