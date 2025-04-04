using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.DTOs;
public class AddFavoriteDTO
{
        public int Favorite__ProductColorId { get; set; }
        public DateTime Favorite__CreatedDate { get; set; } = DateTime.UtcNow;
        public bool Favorite__IsDeleted { get; set; } = false;
}

    public class GetFavoriteDTO
    {
        public int Favorite__Id { get; set; }
        public string? Color {get;set;}
        public string? Product__Name {get;set;}
        public int Favorite__CreatedByAccountId { get; set; }
        public ProductColor? ProductColor {get;set;}
        public List<ProductColorFile>? Images {get;set;}
        public bool? Favorite__IsSale {get;set;}=true;
        public string? Favorite__Message {get;set;}
    }

