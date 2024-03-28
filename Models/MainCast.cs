namespace Rihal_Cinema.Models
{
    public class MainCast : BaseModel
    {
        public string Name { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
