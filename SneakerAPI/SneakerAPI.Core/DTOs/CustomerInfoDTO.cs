

using Microsoft.AspNetCore.Http;

namespace SneakerAPI.Core.DTOs
{
    public class CustomerInfoDTO
    {   
        public int CustomerInfo__Id { get; set; }
        public string? CustomerInfo__FirstName { get; set; }
        public string? CustomerInfo__LastName { get; set; }
        public string? CustomerInfo__Phone { get; set; }
        public string? CustomerInfo__Avatar { get; set; }
        public decimal CustomerInfo__TotalSpent { get; set; }
        public decimal CustomerInfo__SpendingPoint { get; set; }
        public int CustomerInfo__AccountId { get; set; }
        public IFormFile? File {get;set;}
    }
}