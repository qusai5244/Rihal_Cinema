using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Helpers;

namespace Rihal_Cinema.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<string>> CreateUser(UserRegisterInputDto input);
    }
}
