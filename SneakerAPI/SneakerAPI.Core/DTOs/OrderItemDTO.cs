using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.DTOs
{
    public class GetOrderItemDTO
    {
        public int OrderItem__Id { get; set; }
        public string? Color {get;set;}
        public string? Product__Name {get;set;}
        public int OrderItem__CreatedByAccountId { get; set; }
        public ProductColor? ProductColor {get;set;}
        public Size? Size { get; set; }
        public List<ProductColorFile>? Images {get;set;}
        public bool? OrderItem__IsSale {get;set;}=true;
        public string? OrderItem__Message {get;set;}
    }
}