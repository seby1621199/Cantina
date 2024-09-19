using DataAccessLayer.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CantinaAPI.Auth
{
    public class Auth : IAuth
    {
        private readonly IConfiguration configuration;
        public Auth(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJWTToken(User user, IList<string> roles)
        {

            var claims = new List<Claim>
            {
                          new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                          new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var jwtToken = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(configuration["Jwt:Key"])
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
