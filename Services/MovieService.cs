using Microsoft.EntityFrameworkCore;
using Rihal_Cinema.Data;
using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Dtos.StarSystem;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Enums;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Infrastructure.ServiceContext;
using Rihal_Cinema.Models;
using Rihal_Cinema.Services.Interfaces;
using System.Linq;

namespace Rihal_Cinema.Services
{
    public class MovieService : IMovieService
    {
        private readonly DataContext _dataContext;
        public RequestHeaderContent Header { get; set; }

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
                    var avgRate = CalculateAverageRating(rates, movie.Id);
                    movie.AverageRating = avgRate == null ? null : (int)Math.Round((decimal)avgRate);
                    movie.Description = TruncateDescription(movie.Description);
                }

                var pagintaedList = new PaginatedList<MoviesOutputDto>(movies, totalMovies, page, pageSize);

                return new ApiResponse<PaginatedList<MoviesOutputDto>>(true, (int)ResponseCodeEnum.Success, "Movies Retrieved successfully", pagintaedList);
            }
            catch (Exception)
            {
                return new ApiResponse<PaginatedList<MoviesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Issue Occurred While Getting Movies Data", null);
            }
        }

        public async Task<ApiResponse<MovieRateDto>> RateMovie(MovieRateDto input)
        {
            try
            {

                var previousRating = await _dataContext
                           .Rates
                           .Where(r => r.UserId == Header.UserId && r.MovieId == input.MovieId)
                           .FirstOrDefaultAsync();

                if (previousRating != null)
                {

                    if(previousRating.Value == input.Rate)
                    {
                        return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "This movie have been rated before, The Rate has updated Successfully", input);
                    }

                    previousRating.Value = input.Rate;
                    previousRating.UpdatedAt= DateTime.UtcNow;
                    await _dataContext.SaveChangesAsync();
                    return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "This movie have been rated before, The Rate has updated Successfully", input);

                }

                var user = await _dataContext
                                 .Users
                                 .AsNoTracking()
                                 .Where(u => u.Id == Header.UserId)
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
                    UserId = Header.UserId,
                    MovieId = input.MovieId,
                    Value = input.Rate
                };

                _dataContext.Rates.Add(newRate);
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<MovieRateDto>(true, (int)ResponseCodeEnum.Success, "Rate Added Successfully", input);

            }
            catch (Exception)
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

                var rates = await _dataContext
                                  .Rates
                                  .AsNoTracking()
                                  .Where(m => m.MovieId == id)
                                  .ToListAsync();
                double? averageRating = null;
                int? myRate = null;

                if (rates.Any())
                {
                    averageRating = rates.Select(r => r.Value).Average();
                    myRate = rates.Where(r => r.UserId == Header.UserId).Select(r => r.Value).FirstOrDefault();
                }


                var movieDto = new MovieOutputDto
                {
                    Id = movie.Id,
                    Name = movie.Name,
                    Description = movie.Description,
                    MainCasts = mainCasts,
                    ReleaseDate = movie.ReleaseDate?.ToString("dd-MM-yyyy"),
                    Director = movie.Director != null ? movie.Director : null,
                    Budget = movie.Budget != null ? NumberToText((int)movie.Budget) : null,
                    MyRate = myRate,
                    AverageRating = averageRating
                };

                return new ApiResponse<MovieOutputDto>(true, (int)ResponseCodeEnum.Success, "Movie Data Retrieved", movieDto);

            }
            catch (Exception)
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

                searchInput = searchInput.ToLower();

                var page = paginationInput.Page > 0 ? paginationInput.Page : 1;
                var pageSize = paginationInput.PageSize > 0 ? paginationInput.PageSize : 10;

                var moviesQuery = _dataContext
                                   .Movies
                                   .AsNoTracking()
                                   .AsQueryable()
                                   .Where(m => m.Name.ToLower().Contains(searchInput) || m.Description.ToLower().Contains(searchInput));

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
            catch (Exception)
            {
                return new ApiResponse<PaginatedList<MoviesSearchOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Issue Occurred While Getting Movies Data", null);
            }
        }

        public async Task<ApiResponse<List<Top5RatedMoviesOutputDto>>> GetMyTopFiveRatedMovies()
        {
            try
            {
                var top5RatedMovies = await _dataContext
                                  .Rates
                                  .Include(r => r.Movie)
                                  .AsNoTracking()
                                  .Where(r => r.UserId == Header.UserId)
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
            catch (Exception)
            {
                return new ApiResponse<List<Top5RatedMoviesOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While getting Top 5 Rated Movies", null);
            }
        }

        public async Task<ApiResponse<ScrambledMoviePuzzleOutputDto>> GuessTheMovie(string scrambledName)
        {
            try
            {
                if (string.IsNullOrEmpty(scrambledName))
                {
                    return new ApiResponse<ScrambledMoviePuzzleOutputDto>(false, (int)ResponseCodeEnum.BadRequest, "Scrambled Name Input cannot be empty", null);
                }

                var movies = await _dataContext.Movies.AsNoTracking().ToListAsync();

                var guessedMovie = movies.FirstOrDefault(m => MatchScrambledNameWithActualName(scrambledName, m.Name));

                if (guessedMovie == null)
                {
                    return new ApiResponse<ScrambledMoviePuzzleOutputDto>(false, (int)ResponseCodeEnum.NotFound, "No Movie matched the scrambled name", null);
                }

                var guessedMovieDto = new ScrambledMoviePuzzleOutputDto
                {
                    Id = guessedMovie.Id,
                    Name = guessedMovie.Name,
                    Description = guessedMovie.Description
                };

                return new ApiResponse<ScrambledMoviePuzzleOutputDto>(true, (int)ResponseCodeEnum.Success, "Movie matched", guessedMovieDto);
            }
            catch (Exception)
            {
                return new ApiResponse<ScrambledMoviePuzzleOutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An error occurred while getting movie", null);
            }
        }

        public async Task<ApiResponse<List<CompareMoviesRatingsOutputDto>>> RatingsCompare()
        {
            try
            {
                // Get Movies i have rated
                var myRatings = await _dataContext
                                    .Rates
                                    .AsNoTracking()
                                    .Where(r => r.UserId == Header.UserId)
                                    .ToListAsync();

                if (!myRatings.Any())
                {
                    return new ApiResponse<List<CompareMoviesRatingsOutputDto>>(false, (int)ResponseCodeEnum.NotFound, "No Ratings Found for This User", null);
                }

                var ratingsMovieIds = myRatings.Select(r => r.MovieId).ToList();

                var movies = await _dataContext
                                   .Movies
                                   .Include(m => m.Rates)
                                   .AsNoTracking()
                                   .Where(m => ratingsMovieIds.Contains(m.Id))
                                   .ToListAsync();

                var rates = movies.SelectMany(r => r.Rates);



                List<CompareMoviesRatingsOutputDto> compareMoviesRatingsOutputDto = new List<CompareMoviesRatingsOutputDto>();

                foreach (var movie in movies)
                {
                    var userMovieRate = myRatings.Where(r => r.MovieId == movie.Id).Select(r => r.Value).FirstOrDefault(); // User Rate
                    var movieRates = (rates.Where(r => r.MovieId == movie.Id).Select(r => r.Value)).ToList();
                    var movieAverageRates = movieRates.Sum() / movieRates.Count;

                    var output = new CompareMoviesRatingsOutputDto
                    {
                        Id = movie.Id,
                        Name = movie.Name,
                        Rating = userMovieRate,
                        IsMaximum = userMovieRate >= movieAverageRates ? true : false,
                    };

                    compareMoviesRatingsOutputDto.Add(output);
                }

                return new ApiResponse<List<CompareMoviesRatingsOutputDto>>(true, (int)ResponseCodeEnum.Success, "Compare Ratings Retrieved", compareMoviesRatingsOutputDto);

            }
            catch (Exception)
            {
                return new ApiResponse<List<CompareMoviesRatingsOutputDto>>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred while Compare Ratings", null);
            }
        }

        public async Task<ApiResponse<StarSystemoutputDto>> StarSystem(List<int> input)
        {
            try
            {
                if (input == null || input.Count == 0)
                {
                    return new ApiResponse<StarSystemoutputDto>(false, (int)ResponseCodeEnum.BadRequest, "movie Ids List Can Not Be Empty",null);
                };


                List<int> moviesRates = new List<int>();
                var movies = await _dataContext
                                   .Movies
                                   .AsNoTracking()
                                   .Where(m => input.Contains(m.Id))
                                   .ToListAsync();

                if (!movies.Any())
                {
                    return new ApiResponse<StarSystemoutputDto>(false, (int)ResponseCodeEnum.NotFound, "movies Not Found", null);
                }

                var rates = await _dataContext
                                .Rates
                                .AsNoTracking()
                                .Where(r => input.Contains(r.MovieId))
                                .ToListAsync();

                if (!rates.Any())
                {
                    return new ApiResponse<StarSystemoutputDto>(false, (int)ResponseCodeEnum.NotFound, "rates Not Found", null);
                }

                List<int> averageRatsList = new List<int>();

                foreach (var movie in movies)
                {
                    var rate = CalculateAverageRating(rates, movie.Id);
                    var intRate = (int)Math.Round((decimal)rate);
                    averageRatsList.Add(intRate);
                    if(rate == null)
                    {
                        moviesRates.Add(0);
                    }
                    else
                    {
                        moviesRates.Add(intRate);
                    }
                }

                var minimumStars = moviesRates.Count;

                // Iterate through the list and print out each element
                for (int i = 0; i < moviesRates.Count; i++)
                {
                    if (i == 0 && moviesRates[i] > moviesRates[i + 1])
                    {
                        minimumStars++;
                    }
                    else if (i == moviesRates.Count - 1 && moviesRates[i] > moviesRates[i - 1])
                    {
                        minimumStars++;
                    }
                    else if (i > 0 && i < moviesRates.Count - 1)
                    {
                        if (moviesRates[i] > moviesRates[i - 1])
                        {
                            minimumStars++;
                        }

                        if (moviesRates[i] > moviesRates[i + 1])
                        {
                            minimumStars++;
                        }
                    }
                }

                var starSystemoutputDto = new StarSystemoutputDto
                {
                    Rates = averageRatsList,
                    MinimumStars = minimumStars,
                };

                return new ApiResponse<StarSystemoutputDto>(true, (int)ResponseCodeEnum.Success, "Minimum Stars Retrieved Successfully", starSystemoutputDto);

            }
            catch (Exception)
            {
                return new ApiResponse<StarSystemoutputDto>(false, (int)ResponseCodeEnum.InternalServerError, "An Error Occurred While Getting Minimum Stars", null);
            }
        }

        // Private Functions
        private static decimal? CalculateAverageRating(List<Rate> rates, int movieId)
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

        private static string NumberToText(int number)
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

        private static string NumberToTextHelper(int number)
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

        private static bool MatchScrambledNameWithActualName(string scrambledName, string movieName)
        {
            // Remove spaces from scrambledName and movieName
            scrambledName = RemoveSpaces(scrambledName);
            movieName = RemoveSpaces(movieName);

            char[] scrambledNameChars = scrambledName.ToLower().ToCharArray();
            char[] movieNameChars = movieName.ToLower().ToCharArray();

            Array.Sort(scrambledNameChars);
            Array.Sort(movieNameChars);

            return new string(scrambledNameChars) == new string(movieNameChars);
        }

        private static string RemoveSpaces(string str)
        {
            // Split the string into words
            string[] words = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // Concatenate the words without spaces
            return string.Join("", words);
        }



    }
}
