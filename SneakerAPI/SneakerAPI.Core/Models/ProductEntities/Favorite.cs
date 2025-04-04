

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Favorite
    {
        public int Favorite__Id { get; set; }
        public int Favorite__AccountId { get; set; }
        public int Favorite__ProductColorId { get; set; }
        public DateTime Favorite__CreatedDate { get; set; } = DateTime.UtcNow;
        public bool Favorite__IsDeleted { get; set; } = false;
        public virtual IdentityAccount? Account { get; set; }
        public virtual ProductColor? ProductColor { get; set; }
    }
}