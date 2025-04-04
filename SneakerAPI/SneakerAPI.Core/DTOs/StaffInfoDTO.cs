

using Microsoft.AspNetCore.Http;

namespace SneakerAPI.Core.DTOs;
    public class StaffInfoDTO
    {   
        public int StaffInfo__Id { get; set; }=13;
        public string? StaffInfo__FirstName { get; set; }
        public string? StaffInfo__LastName { get; set; }
        public string? StaffInfo__Phone { get; set; }
        public string? StaffInfo__Avatar { get; set; }
        public int StaffInfo__AccountId { get; set; }
        public IFormFile? File {get;set;}
}