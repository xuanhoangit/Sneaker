

namespace SneakerAPI.Core.DTOs
{   
    
    public class AddressDTO
    {   

        public int Address__Id { get; set; }
        public string Address__AddressDetail { get; set; }
        public string Address__Phone { get; set; }
        public bool? Address__IsDefault { get; set; }
        
        public string Address__WardCode { get; set; }
        public int Address__DistrictId { get; set; }
        public string Address__ProvinceName { get; set; }
        public string Address__ReceiverName { get; set; }
        public int Address__CustomerInfoId { get; set; }
        

    }
}