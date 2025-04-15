

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Favorite
    {   
        [Key]
        public int Favorite__Id { get; set; }
        public int Favorite__AccountId { get; set; }
        public int Favorite__ProductId { get; set; }
        public DateTime Favorite__CreatedDate { get; set; } = DateTime.UtcNow;
        public bool Favorite__IsDeleted { get; set; } = false;
        [ForeignKey("Favorite__AccountId")]
        public virtual IdentityAccount? Account { get; set; }
        [ForeignKey("Favorite__ProductId")]
        public virtual Product? Product { get; set; }
    }
}