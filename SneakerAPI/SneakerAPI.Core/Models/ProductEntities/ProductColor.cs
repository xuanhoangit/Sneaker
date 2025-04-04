
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class ProductColor
    {   
        [Key]
        public int ProductColor__Id { get; set; }
        public decimal ProductColor__Price { get; set; }
        public required string ProductColor__Name { get; set; }
        public string? ProductColor__Description { get; set; }
        public int ProductColor__Status {get;set;} = (int)Status.Unreleased;
        public int ProductColor__ColorId { get; set; }
        [ForeignKey("ProductColor__ColorId")]
        public  Color? Color { get; set; }
        public int ProductColor__ProductId { get; set; }
        [ForeignKey("ProductColor__ProductId")]
        public  Product? Product { get; set; }
        // [NotMapped]
        // public virtual ICollection<ProductColorSize>? ProductColorSizes { get; set; }
    }
}