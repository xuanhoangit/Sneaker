using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Size
    {   
        [Key]
        public int Size__Id { get; set; }
        public string? Size__Value { get; set; }
    }
}