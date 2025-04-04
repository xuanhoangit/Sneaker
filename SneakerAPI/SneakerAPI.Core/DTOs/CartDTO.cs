using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.DTOs;
public class AddCartDTO
{
        public int CartItem__Id { get; set; }
        public int CartItem__Quantity {get;set;}
        public int CartItem__ProductColorSizeId { get; set; }//FK

}


    public class GetCartItemDTO
    {
        public int CartItem__Id { get; set; }
        public string? Color {get;set;}
        public string? Product__Name {get;set;}
        public int CartItem__Quantity {get;set;}
        public int CartItem__CreatedByAccountId { get; set; }
        public int CartItem__ProductColorSizeId {get;set;}
        public ProductColor? ProductColor {get;set;}
        public Size? Size { get; set; }
        public List<ProductColorFile>? Images {get;set;}
        public bool? CartItem__IsSale {get;set;}=true;
        public string? CartItem__Message {get;set;}
    }
