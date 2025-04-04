using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class ProductCategory
    {   
        [Key]
        public int ProductCategory__Id { get; set; }
        public int ProductCategory__ProductId { get; set; }
        [ForeignKey ("ProductCategory__ProductId")]
        public  Product? Product { get; set; }
        [ForeignKey ("ProductCategory__CategoryId")]
        public int ProductCategory__CategoryId { get; set; }
        public  Category? Category { get; set; }
    }
}