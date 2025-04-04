namespace SneakerAPI.Core.DTOs
{
    public class ProductDTO
    {
        public  int Product__Id { get; set; }
        public required string Product__Name { get; set; }
        public required string Product__Description { get; set; }
        public required int Product__CreatedByAccountId { get; set; }  
        public required int Product__BrandId { get; set; }
    }
}
