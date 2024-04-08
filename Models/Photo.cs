namespace Rihal_Cinema.Models
{
    public class Photo : BaseModel
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public decimal Size { get; set; }
        public string StoredName { get; set; }
        public int MemoryId { get; set; }
        public Memory Memory { get; set; }
    }
}
