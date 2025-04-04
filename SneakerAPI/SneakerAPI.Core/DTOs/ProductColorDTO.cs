namespace SneakerAPI.Core.DTOs
{
    public class ProductColorDTO
    {
        public int ProductColor__Id { get; set; }
        public string? ProductColor__Name { get; set; }
        public decimal ProductColor__Price { get; set; }
        public int ProductColor__Status {get;set;} = (int)Status.Unreleased;
        public int ProductColor__ColorId { get; set; }
        public string? ProductColor__Description { get; set; }
        public int ProductColor__ProductId { get; set; }
    }
}