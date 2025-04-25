using Microsoft.AspNetCore.Http;

namespace SneakerAPI.Core.DTOs;

public class AddBrandDTO
{
    
        public string Brand__Name { get; set; }
        public string? Brand__Description { get; set; }
        public bool Brand__IsActive { get; set; }
        public IFormFile? FileLogo { get; set; }
}