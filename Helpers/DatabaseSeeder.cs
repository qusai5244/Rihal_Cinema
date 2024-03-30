using Rihal_Cinema.Data;
using Rihal_Cinema.Services.Interfaces;

namespace Rihal_Cinema.Helpers
{
    public class DatabaseSeeder
    {
        private readonly DataContext _context;
        private readonly ICallRihalApiService _callRihalApiService;

        public DatabaseSeeder(DataContext context, ICallRihalApiService callRihalApiService)
        {
            _context = context;
            _callRihalApiService = callRihalApiService;
        }

        public async Task SeedAsync()
        {
            if (!_context.Movies.Any())
            {
                // Call the service to get movie data
                var movies = await _callRihalApiService.GetMoviesByIdsAsync();

                // Seed data from service response
                _context.Movies.AddRange(movies);
                await _context.SaveChangesAsync();
            }

        }
    }
}
