using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Rihal_Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Rihal_Cinema.Handler
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly DataContext _dataContext;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            DataContext dataContext)
            : base(options, loggerFactory, encoder, clock)
        {
            _dataContext = dataContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            try
            {
                var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
                string[] credentials = Encoding.UTF8.GetString(bytes).Split(':');
                string username = credentials[0];
                string password = credentials[1];

                var user = await _dataContext
                                 .Users
                                 .Where(u=> u.Email == username)
                                 .FirstOrDefaultAsync();

                var decodedPassword = Encoding.UTF8.GetString(user.Password);

                if (user is null || decodedPassword != password) 
                {
                    return AuthenticateResult.Fail("Invalid username or password");
                }

                var claims = new[] {
                new Claim(ClaimTypes.Name, username),
                new Claim("id", user.Id.ToString())
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);

            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
