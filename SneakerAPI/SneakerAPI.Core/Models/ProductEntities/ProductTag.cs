using System.ComponentModel.DataAnnotations;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class ProductTag
    {   
        [Key]
        public int ProductTag__Id { get; set; }
        public int ProductTag__ProductId { get; set; }
        public  Product? Product { get; set; }
        public int ProductTag__TagId { get; set; }
        public  Tag? Tag { get; set; }
    }
}