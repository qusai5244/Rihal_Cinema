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

        public async Task<ApiResponse<PaginatedList<MoviesOutputDto>>> GetMovies(PaginationInputDto input)
        {
            try
            {

                var page = input.Page > 0 ? input.Page : 1;
                var pageSize = input.PageSize > 0 ? input.PageSize : 10;

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
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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

                var pagintaedList = new PaginatedList<MoviesOutputDto>(movies, totalMovies, page, pageSize);

                return new ApiResponse<PaginatedList<MoviesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Movies Retrieved successfully", pagintaedList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<MoviesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Issue Occurred While Getting Movies Data", null);
            }
        }

        public async Task<ApiResponse<MovieRateDto>> RateMovie(int userId, MovieRateDto input)
        {
            try
            {

                var previousRating = await _dataContext
                           .Rates
                           .Where(r => r.UserId == userId && r.MovieId == input.MovieId)
                           .FirstOrDefaultAsync();

                if (previousRating != null)
                {

                    if(previousRating.Value == input.Rate)
                    {
                        return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "This movie have been rated before, The Rate has updated Successfully", input);
                    }

                    previousRating.Value = input.Rate;
                    await _dataContext.SaveChangesAsync();
                    return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "This movie have been rated before, The Rate has updated Successfully", input);

                }

                var user = await _dataContext
                                 .Users
                                 .AsNoTracking()
                                 .Where(u => u.Id == userId)
                                 .FirstOrDefaultAsync();

                if (user == null)
                {
                    return new ApiResponse<MovieRateDto>(false, (int)ResponseCodeEnum.NotFound, "User Not Found", null);
                }

                var movie = await _dataContext
                                  .Movies
                                  .AsNoTracking()
                                  .Where(m => m.Id == input.MovieId)
                                  .FirstOrDefaultAsync();

                if (movie == null)
                {
                    return new ApiResponse<MovieRateDto>(false, (int)ResponseCodeEnum.NotFound, "Movie Not Found", null);
                }



                var newRate = new Rate
                {
                    UserId = userId,
                    MovieId = input.MovieId,
                    Value = input.Rate
                };

                _dataContext.Rates.Add(newRate);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "Rate Added Successfully", input);

            }
            catch (Exception ex)
            {
                return new ApiResponse<MovieRateDto>(false, (int)ResponseCodeEnum.InternalServerError, " An Error Occurred While Adding Rating Movie", null);
            }
        }

        public async Task<ApiResponse<MovieOutputDto>> GetMovie(int id)
        {
            try
            {
                var movie = await _dataContext
                                  .Movies
                                  .Include(m => m.MainCasts)
                                  .AsNoTracking()
                                  .Where(m => m.Id == id)
                                  .FirstOrDefaultAsync();

                if (movie == null)
                {
                    return new ApiResponse<MovieOutputDto>(false, (int)ResponseCodeEnum.NotFound, "Movie Not Found", null);
                }

                List<string> mainCasts = movie.MainCasts.Select(m => m.Name).ToList();

                var x = movie.ReleaseDate?.ToString("dd-MM-yyyy");

                var movieDto = new MovieOutputDto
                {
                    Id = movie.Id,
                    Name = movie.Name,
                    Description = movie.Description,
                    MainCasts = mainCasts,
                    ReleaseDate = movie.ReleaseDate?.ToString("dd-MM-yyyy"),
                    Director = movie.Director != null ? movie.Director : null,
                    Budget = movie.Budget != null ? NumberToText((int)movie.Budget) : null
                };

                return new ApiResponse<MovieOutputDto>(true, (int)ResponseCodeEnum.Success, "Movie Data Retrieved", movieDto);

            }
            catch (Exception ex)
            {
                return new ApiResponse<MovieOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while getting Movie data", null);
            }
        }

        public async Task<ApiResponse<PaginatedList<MoviesSearchOutputDto>>> MoviesSearch(string searchInput, PaginationInputDto paginationInput)
        {
            try
            {
                if (String.IsNullOrEmpty(searchInput))
                {
                    return new ApiResponse<PaginatedList<MoviesSearchOutputDto>> (false, (int)ResponseCodeEnum.BadRequest, "Search Input can not be Empty", null);
                }

                var page = paginationInput.Page > 0 ? paginationInput.Page : 1;
                var pageSize = paginationInput.PageSize > 0 ? paginationInput.PageSize : 10;

                var moviesQuery = _dataContext
                                   .Movies
                                   .AsNoTracking()
                                   .AsQueryable()
                                   .Where(m => m.Name.Contains(searchInput) || m.Description.Contains(searchInput));

                var moviesCount = await moviesQuery.CountAsync();

                var moviesDto = await moviesQuery
                                .Select(m => new MoviesSearchOutputDto
                                {
                                    Id = m.Id,
                                    Name = m.Name,
                                    Description = m.Description,
                                })
                                .Skip((paginationInput.Page - 1) * paginationInput.PageSize)
                                .Take(paginationInput.PageSize)
                                .ToListAsync();

                if (!moviesDto.Any()) 
                {
                    return new ApiResponse<PaginatedList<MoviesSearchOutputDto>>(false, (int)ResponseCodeEnum.NotFound, "No Movies Were Found That Contain the Search Input", null);
                }

                var pagintaedList = new PaginatedList<MoviesSearchOutputDto>(moviesDto, moviesCount, page, pageSize);


                return new ApiResponse<PaginatedList<MoviesSearchOutputDto>>(true, (int)ResponseCodeEnum.Success, "Movies Were Found That Contain the Search Input", pagintaedList);

            }
            catch (Exception ex)
            {
                return new ApiResponse<PaginatedList<MoviesSearchOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Issue Occurred While Getting Movies Data", null);
            }
        }

        public async Task<ApiResponse<List<Top5RatedMoviesOutputDto>>> GetMyTopFiveRatedMovies(int userId)
        {
            try
            {
                var top5RatedMovies = await _dataContext
                                  .Rates
                                  .Include(r => r.Movie)
                                  .AsNoTracking()
                                  .Where(r => r.UserId == userId)
                                  .OrderByDescending(r => r.Value)
                                  .Take(5)
                                  .Select(r => new Top5RatedMoviesOutputDto
                                  {
                                      Id = r.Movie.Id,
                                      Name = r.Movie.Name,
                                      Rating = r.Value
                                  })
                                  .ToListAsync();

                if (!top5RatedMovies.Any())
                {
                    return new ApiResponse<List<Top5RatedMoviesOutputDto>>(false, (int)ResponseCodeEnum.NotFound, "No Rated Movies Were Found", null);
                }

                return new ApiResponse<List<Top5RatedMoviesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Top 5 Rated Movies Retrieved", top5RatedMovies);


            }
            catch (Exception ex)
            {
                return new ApiResponse<List<Top5RatedMoviesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While getting Top 5 Rated Movies", null);
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

        private string NumberToText(int number)
        {
            string[] thousands = { "", "thousand", "million", "billion", "trillion" };

            if (number == 0)
                return "zero";

            string result = "";

            int count = 0;
            while (number > 0)
            {
                if (number % 1000 != 0)
                {
                    result = $"{NumberToTextHelper(number % 1000)} {thousands[count]} {result}";
                }
                number /= 1000;
                count++;
            }

            return result.Trim();
        }

        private string NumberToTextHelper(int number)
        {

            string[] units = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            string[] teens = { "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tens = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
            string result = "";

            if (number >= 100)
            {
                result += units[number / 100] + " hundred ";
                number %= 100;
            }

            if (number >= 10 && number <= 19)
            {
                result += teens[number - 10];
                return result;
            }
            else if (number >= 20)
            {
                result += tens[number / 10];
                number %= 10;
            }

            if (number >= 1 && number <= 9)
            {
                result += units[number];
            }

            return result;
        }

    }
}
