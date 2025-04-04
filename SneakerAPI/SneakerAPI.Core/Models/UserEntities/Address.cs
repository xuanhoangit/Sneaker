using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.UserEntities
{
    public class Address
    {   
        [Key]
        public int Address__Id { get; set; }
        public string? Address__FullAddress { get; set; }
        public string? Address__Phone { get; set; }
        public bool? Address__IsDefault { get; set; }
        public string? Address__ReceiverName { get; set; }
        public int Address__CustomerInfo { get; set; }
        [ForeignKey("Address__CustomerInfo")]
        public CustomerInfo? CustomerInfo { get; set; }
    }
}