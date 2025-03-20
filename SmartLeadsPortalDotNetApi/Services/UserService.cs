using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartLeadsPortalDotNetApi.Entities;
using SmartLeadsPortalDotNetApi.Helper;
using SmartLeadsPortalDotNetApi.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartLeadsPortalDotNetApi.Services
{
    public interface IUserService
    {
        //AuthenticateResponse Authenticate(User user);
        //AuthenticateResponse ReAuthenticate(User user);
        //User AuthenticateUser(AuthenticateRequest model);
        //User ReAuthenticateUser(AuthenticateRequest model);
    }
    public class UserService : SQLDBService, IUserService
    {
        private readonly AppSettings _appSettings;
        public UserService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public AuthenticateResponse Authenticate(User user)
        {

            if (user == null) return null;

            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }
        private string generateJwtToken(User user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.id.ToString()), new Claim("roleid", user.userRoleId.ToString()) }),
                Expires = DateTime.UtcNow.AddHours(9),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
