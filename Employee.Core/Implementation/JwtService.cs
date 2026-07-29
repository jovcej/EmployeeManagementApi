using Employee.Core.Abstraction;
using Employee.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Employee.Core.Implementation
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim("sub", user.Id.ToString()),
                new Claim("username", user.Username.Value),
                new Claim("role", user.Role.ToString())
            };

            var jwtKey = _configuration["Jwt:Key"]
                    ?? throw new Exception("JWT Key is missing in configuration.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var  token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims:claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
