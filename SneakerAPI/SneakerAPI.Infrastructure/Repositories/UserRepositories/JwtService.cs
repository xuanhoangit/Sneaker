using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.Models;
using SneakerAPI.Infrastructure.Data;
namespace SneakerAPI.Infrastructure.Repositories.UserRepositories;
public class JwtService : IJwtService
    {
    private readonly IConfiguration config;

    public JwtService(IConfiguration _config)
        {
        config = _config;
    }
        public object GenerateJwtToken(IdentityAccount account, IList<string> roles)
        {
            try
            {
                
            var jwtSettings = config.GetSection("JWT");
            System.Console.WriteLine("jwtSettings"+jwtSettings["ValidIssuer"]);
            System.Console.WriteLine("jwtSettings"+jwtSettings["ValidAudience"]);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
           
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),//Sub sẽ đại diện cho Identifier 
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: credentials
            );
            return new TokenResponse{
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken= Guid.NewGuid().ToString()
                };
             }
            catch (System.Exception)
            {
                
                throw new Exception("An error occured while generating token");
            }
        }
        
}