using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.DTOs;
public class AddFavoriteDTO
{
        public int Favorite__ProductId { get; set; }
}

    public class GetFavoriteDTO
    {   
        public int Favorite__ProductId { get; set; }
        public int Favorite__Id { get; set; }
        public string? Color {get;set;}
        public string? Product__Name {get;set;}
        public int Favorite__CreatedByAccountId { get; set; }
        public ProductColor? ProductColor {get;set;}
        public ProductColorFile? Image{get;set;}
        public bool? Favorite__IsSale {get;set;}=true;
        public string? Favorite__Message {get;set;}
    }

