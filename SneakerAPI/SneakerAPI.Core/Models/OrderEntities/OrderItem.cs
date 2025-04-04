using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.Models.OrderEntities
{
    public class OrderItem
    {   
        [Key]
        public int OrderItem__Id { get; set; }
        public int OrderItem__OrderId { get; set; }//FK
        [ForeignKey("OrderItem__OrderId")]
        public Order? Order { get; set; }
        public int OrderItem__ProductColorSizeId { get; set; }//FK
        [ForeignKey("OrderItem__ProductColorSizeId")]
        public ProductColorSize? ProductColorSize { get; set; }
        public int OrderItem__Quantity { get; set; }
    }
}
