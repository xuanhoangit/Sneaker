using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class ProductColorSize
    {   
        [Key]
        public int ProductColorSize__Id { get; set; }
        public int ProductColorSize__Quantity { get; set; }

        public int ProductColorSize__SizeId { get; set; }
        [ForeignKey("ProductColorSize__SizeId")]
        public  Size? Size { get; set; }
        public int ProductColorSize__ProductColorId { get; set; }
        [ForeignKey("ProductColorSize__ProductColorId")]
        public  ProductColor? ProductColor { get; set; }
    }
}

