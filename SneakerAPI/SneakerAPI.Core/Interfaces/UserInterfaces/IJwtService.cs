namespace SneakerAPI.Core.Interfaces.UserInterfaces;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models;

public interface IJwtService 
{
    object GenerateJwtToken(IdentityAccount account, IList<string> roles);
}