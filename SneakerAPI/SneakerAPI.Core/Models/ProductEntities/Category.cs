using System.ComponentModel.DataAnnotations;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Category
    {   
        [Key]
        public int Category__Id { get; set; }
        public string? Category__Name { get; set; }
        public string? Category__Description { get; set; }
        public virtual List<ProductCategory>? ProductCategories {get;set;}
    }
}