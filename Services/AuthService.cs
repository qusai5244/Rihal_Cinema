using Microsoft.EntityFrameworkCore;
using Rihal_Cinema.Data;
using Rihal_Cinema.Dtos.User;
using Rihal_Cinema.Enums;
using Rihal_Cinema.Helpers;
using Rihal_Cinema.Models;
using Rihal_Cinema.Services.Interfaces;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Rihal_Cinema.Services
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _dataContext;

        public AuthService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ApiResponse<string>> CreateUser(UserRegisterInputDto input)
        {
            try
            {
                var emailExist = await _dataContext
                                       .Users
                                       .AsNoTracking()
                                       .Where(u => u.Email == input.Email)
                                       .AnyAsync();

                if (emailExist)
                {
                    return new ApiResponse<string>(false, (int)ResponseCodeEnum.Success, "Email Already Exist", null);
                }

                // Encode password to byte array
                byte[] encodedPassword = Encoding.UTF8.GetBytes(input.Password);

                // Create new user object
                var user = new User
                {
                    Email = input.Email,
                    Password = encodedPassword,
                };

                // Add user to DbSet
                _dataContext.Users.Add(user);

                // Save changes to database
                await _dataContext.SaveChangesAsync();

                return new ApiResponse<string>(true, (int)ResponseCodeEnum.Success, "New User Created", null);
            }
            catch (Exception)
            {
                // Handle error
                return new ApiResponse<string>(false, (int)ResponseCodeEnum.InternalServerError, "Error Occurred while creating new User", null);
            }
        }
    }
}
