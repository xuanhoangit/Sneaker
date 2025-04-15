using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.UserEntities
{
    public class Address
    {   
        [Key]
        public int Address__Id { get; set; }
        public required string Address__AddressDetail { get; set; }

        public required string Address__Phone { get; set; }
        public required bool? Address__IsDefault { get; set; }
        
        public required string Address__WardCode { get; set; }
        public required int Address__DistrictId { get; set; }
        public required string Address__ProvinceName { get; set; }
        public required string Address__ReceiverName { get; set; }
        public int Address__CustomerInfo { get; set; }
        [ForeignKey("Address__CustomerInfo")]
        public CustomerInfo? CustomerInfo { get; set; }
    }
}