using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SneakerAPI.Core.Libraries;

namespace SneakerAPI.Core.Models.UserEntities
{
    public class CustomerInfo
    {   
        [Key]
        public int CustomerInfo__Id { get; set; }
        public string? CustomerInfo__FirstName { get; set; }
        public string? CustomerInfo__LastName { get; set; }
        // public string? CustomerInfo__Ward { get; set; }
        // public string? CustomerInfo__District { get; set; }
        // public string? CustomerInfo__Province { get; set; }
        public string? CustomerInfo__Phone { get; set; }
        [DefaultValue(HandleString.DefaultImage)]
        public string? CustomerInfo__Avatar { get; set; }
        public decimal CustomerInfo__TotalSpent { get; set; }
        public decimal CustomerInfo__SpendingPoint { get; set; }
        // [Unique]
        public int CustomerInfo__AccountId { get; set; }
        [ForeignKey("CustomerInfo__AccountId")]
        public IdentityAccount? Account { get; set; }
    }
}