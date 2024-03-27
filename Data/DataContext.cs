using Microsoft.EntityFrameworkCore;

namespace Rihal_Cinema.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}
