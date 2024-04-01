using Microsoft.EntityFrameworkCore;
using Rihal_Cinema.Data;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Enums;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Models;
using Rihal_Cinema.Services.Interfaces;
using System.Linq;

namespace Rihal_Cinema.Services
{
    public class MovieService : IMovieService
    {
        private readonly DataContext _dataContext;

        public MovieService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<PaginatedList<MoviesOutputDto>>> GetMovies(SearchAndPaginationInputDto input)
        {
            try
            {

                var moviesQuery = _dataContext
                                    .Movies
                                    .AsNoTracking()
                                    .Select(m => new MoviesOutputDto
                                    {
                                        Id = m.Id,
                                        Name = m.Name,
                                        Description = m.Description,
                                    });

                var totalMovies = await moviesQuery.CountAsync();

                var movies = await moviesQuery
                    .Skip((input.Page - 1) * input.PageSize)
                    .Take(input.PageSize)
                    .ToListAsync();



                var moviesIds = movies.Select(m => m.Id).ToList(); 

                var rates = await _dataContext
                                  .Rates
                                  .AsNoTracking()
                                  .Where(r => moviesIds.Contains(r.MovieId))
                                  .ToListAsync();

                foreach (var movie in movies)
                {
                    movie.AverageRating = CalculateAverageRating(rates, movie.Id);
                    movie.Description = TruncateDescription(movie.Description);
                }

                var pagintaedList = new PaginatedList<MoviesOutputDto>(movies, totalMovies, input.Page, input.PageSize);

                return new ApiResponse<PaginatedList<MoviesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Movies fetched successfully", pagintaedList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<MoviesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Issue Occurred While Fetching Movies", null);
            }
        }

        private decimal? CalculateAverageRating(List<Rate> rates, int movieId)
        {
            var ratings = rates.Where(r => r.MovieId == movieId).Select(r => r.Value);
            return ratings.Any() ? (decimal?)ratings.Average() : null;
        }

        private static string TruncateDescription(string description)
        {
            var maxLength = 100;
            // Check if the description length is already within the limit
            if (description.Length <= maxLength)
                return description;

            // Truncate the description to the nearest space before or at the maxLength
            int lastSpaceIndex = description.LastIndexOf(' ', maxLength);
            string truncated = description.Substring(0, lastSpaceIndex >= 0 ? lastSpaceIndex : maxLength);

            // Add "..." to the end
            truncated += "...";

            return truncated;
        }



    }
}
