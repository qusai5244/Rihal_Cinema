using Rihal_Cinema.Data;

namespace Rihal_Cinema.Services
{
    public class AuthService
    {
        private readonly DataContext _dataContext;

        public AuthService(DataContext dataContext) 
        {  
            _dataContext = dataContext; 
        }


    }
}
