﻿using Rihal_Cinema.Dtos.Photo;

namespace Rihal_Cinema.Dtos.Memory
{
    public class MemoryOutputDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieName { get; set; }
        public string Title { get; set; }
        public string Story { get; set; }
        public List<PhotoListOutputDto> Photos { get; set; }
    }
}
