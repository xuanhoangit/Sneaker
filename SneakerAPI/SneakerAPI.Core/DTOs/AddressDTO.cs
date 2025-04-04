

namespace SneakerAPI.Core.DTOs
{
    public class AddressDTO
    {   

        public int Address__Id { get; set; }
        public string? Address__FullAddress { get; set; }
        public string? Address__Phone { get; set; }
        public bool? Address__IsDefault { get; set; }
        public string? Address__ReceiverName { get; set; }
        public int? Address__CustomerInfo { get; set; }

    }
}