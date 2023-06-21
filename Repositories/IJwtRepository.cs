using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using reservationApi.Models;

namespace reservationApi.Repositories
{
    public interface IJwtRepository
    {
        public Tokens createToken(Customer customer);
        public string createRefreshToken();
    }
    public class JwtRepository : IJwtRepository
    {
        private readonly IConfiguration _config;
        public JwtRepository(IConfiguration config)
        {
            _config = config;
        }

        public string createRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var res = Convert.ToBase64String(randomNumber);

                return res;
            }

        }

        public Tokens createToken(Customer customer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(_config["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                  {
                    new Claim("id", customer.personId.ToString()),
                    new Claim("name", customer.name),
                    new Claim("family", customer.family),
                    new Claim("password", customer.pass),
                    new Claim("email", customer.email),

                  }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var rf = createRefreshToken();
            return new Tokens { accessToken = tokenHandler.WriteToken(token), refreshToken = rf };
        }
    }
}
