﻿namespace Rihal_Cinema.Dtos.Movie
{
    public class MoviesOutputDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? AverageRating { get; set; }
    }
}
