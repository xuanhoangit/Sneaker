using System.ComponentModel.DataAnnotations;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Tag
    {   
        [Key]
        public int Tag__Id { get; set; }
        public string? Tag__Name { get; set; }
    }
}
