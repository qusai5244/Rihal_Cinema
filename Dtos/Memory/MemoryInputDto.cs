using System.ComponentModel.DataAnnotations;

namespace Rihal_Cinema.Dtos.Memory
{
    public class MemoryInputDto : AddPhotoToMemory
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public string Title { get; set; }
        public DateTime TakenOn { get; set; }
        public string Story { get; set; }

    }

    
}
