using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Helpers;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface IMovieService
    {
        Task<ApiResponse<MovieOutputDto>> GetMovie(int id);
        Task<ApiResponse<PaginatedList<MoviesOutputDto>>> GetMovies(PaginationInputDto input);
        Task<ApiResponse<List<Top5RatedMoviesOutputDto>>> GetMyTopFiveRatedMovies(int userId);
        Task<ApiResponse<ScrambledMoviePuzzleOutputDto>> GuessTheMovie(string scrambledName);
        Task<ApiResponse<PaginatedList<MoviesSearchOutputDto>>> MoviesSearch(string searchInput, PaginationInputDto input);
        Task<ApiResponse<MovieRateDto>> RateMovie(int userId, MovieRateDto input);
        Task<ApiResponse<List<CompareMoviesRatingsOutputDto>>> RatingsCompare(int userId);
    }
}
