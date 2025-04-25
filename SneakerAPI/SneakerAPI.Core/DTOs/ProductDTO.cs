using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.DTOs
{
    public class ProductDTO
    {
        public  int? Product__Id { get; set; }
        public required string Product__Name { get; set; }
        public required string Product__Description { get; set; }
        public required int Product__CreatedByAccountId { get; set; }  
        public required int Product__BrandId { get; set; }
    }
     public class GetProductDTO
    {   
        public string? Product__Description {get;set;}
        public int Product__Id { get; set; }
        public string? Product__Name {get;set;}
        public int Product__Sold { get; set; }
        public List<GetProductColorDTO>? ProductColors { get; set; }
    }
}
