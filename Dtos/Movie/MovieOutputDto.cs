namespace Rihal_Cinema.Dtos.Movie
{
    public class MovieOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ReleaseDate { get; set; }
        public List<String> MainCasts { get; set; }
        public string Director { get; set; }
        public string Budget { get; set; }

    }
}
