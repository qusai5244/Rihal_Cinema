using Rihal_Cinema.Dtos;
using Rihal_Cinema.Models;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface ICallRihalApiService
    {
        Task<List<Movie>> GetMoviesByIdsAsync();
    }
}
