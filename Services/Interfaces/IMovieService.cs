using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Dtos.StarSystem;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Infrastructure.ServiceContext;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface IMovieService : IRequestHeader
    {
        Task<ApiResponse<MovieOutputDto>> GetMovie(int id);
        Task<ApiResponse<PaginatedList<MoviesOutputDto>>> GetMovies(PaginationInputDto input);
        Task<ApiResponse<List<Top5RatedMoviesOutputDto>>> GetMyTopFiveRatedMovies();
        Task<ApiResponse<ScrambledMoviePuzzleOutputDto>> GuessTheMovie(string scrambledName);
        Task<ApiResponse<PaginatedList<MoviesSearchOutputDto>>> MoviesSearch(string searchInput, PaginationInputDto input);
        Task<ApiResponse<MovieRateDto>> RateMovie(MovieRateDto input);
        Task<ApiResponse<List<CompareMoviesRatingsOutputDto>>> RatingsCompare();
        Task<ApiResponse<StarSystemoutputDto>> StarSystem(List<int> movieIds);
    }
}
