using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SneakerAPI.Core.Models.UserEntities;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Product
    {   
        [Key]
        public int Product__Id { get; set; }
        public int Product__Sold {get;set;}
        [Required]
        public string? Product__Name { get; set; }
        public string? Product__Description { get; set; }
        public DateTime? Product__CreatedDate { get; set; }=DateTime.UtcNow;
        public DateTime? Product__UpdatedDate { get; set; }
        public int Product__Status {get;set;}=(int)Status.Unreleased;
        // public 
        //
        public int Product__CreatedByAccountId { get; set; } 
        [ForeignKey("Product__CreatedByAccountId")]
        public  IdentityAccount?  Account { get; set; }
        public int Product__BrandId { get; set; }
        [ForeignKey("Product__BrandId")]
        public  Brand? Brand { get; set; }
        // [NotMapped]
        public virtual ICollection<ProductColor> ProductColors { get; set; }= new List<ProductColor>();
    }
}