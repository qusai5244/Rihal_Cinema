namespace Rihal_Cinema.Dtos.Movie
{
    public class CompareMoviesRatingsOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public bool IsMaximum { get; set; }
    }
}
