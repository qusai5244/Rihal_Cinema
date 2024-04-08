namespace Rihal_Cinema.Dtos.Memory
{
    public class MemoriesOutputDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string Title { get; set; }
        public DateTime TakenOn { get; set; }
        public string Story { get; set; }
    }
}
