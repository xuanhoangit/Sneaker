using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.Models.OrderEntities
{
    public class CartItem
    {   
        [Key]
        public int CartItem__Id { get; set; }
        [DefaultValue(1)]
        [Range(1,int.MaxValue)]
        public int CartItem__Quantity {get;set;}
        public int CartItem__CreatedByAccountId { get; set; }=5;
        [ForeignKey("CartItem__CreatedByAccountId")]
        public IdentityAccount? Account { get; set; }
        public int CartItem__ProductColorSizeId { get; set; }//FK
        public ProductColorSize? ProductColorSize {get;set;}
        //Nếu ánh xạ sẽ bị lỗi bởi nếu xóa pc => xóa pcs => xóacart
    }
}