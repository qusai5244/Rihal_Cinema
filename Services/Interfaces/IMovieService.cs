using Rihal_Cinema.Dtos;
using Rihal_Cinema.Dtos.Movie;
using Rihal_Cinema.Helpers;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface IMovieService
    {
        Task<ApiResponse<PaginatedList<MoviesOutputDto>>> GetMovies(SearchAndPaginationInputDto input);
    }
}
