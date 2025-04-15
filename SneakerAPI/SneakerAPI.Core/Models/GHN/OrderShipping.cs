namespace SneakerAPI.Core.Models.GHN
{
    public class OrderShipping
    {
        public string OrderItem__ProductName { get; set; }
        public decimal OrderItem__Price  { get; set; }       
        public decimal OrderItem__Quantity { get; set; }  
        public decimal  OrderItem__TotalPrice { get; set; }  
    }
}