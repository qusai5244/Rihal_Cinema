using System;

namespace Rihal_Cinema.Models
{
    public class Movie : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Director {  get; set; }
        public int? Budget { get; set; }

        public ICollection<Memory> Memories { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<MainCast> MainCasts { get; set; }

    }
}
