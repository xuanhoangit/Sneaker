using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public BaseController(IUnitOfWork uow)
        {
            _uow = uow;
        }
        protected object CurrentUser()
        {   
            try
            {
                
            var account = HttpContext.User;

            if (account == null || !account.Identity.IsAuthenticated)
            {
                return null;
            }

            var sub = account.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email=account.FindFirst(ClaimTypes.Email)?.Value;
            var roles = account.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("role"))
                .Select(c => c.Value)
                .ToList();
            return new CurrentUser
            {
                AccountId = int.Parse(sub),
                Email = email,
                Roles = roles,
            };
            }
        
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}