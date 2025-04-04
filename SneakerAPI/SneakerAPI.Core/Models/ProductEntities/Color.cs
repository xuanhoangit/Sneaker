using System.ComponentModel.DataAnnotations;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Color
    {   
        [Key]
        public int Color__Id { get; set; }
        [Required]
        public string? Color__Name { get; set; }
        [Required]
        public string? Color__Description {get;set;}
    }
}