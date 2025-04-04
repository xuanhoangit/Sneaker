using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SneakerAPI.Core.Libraries;

namespace SneakerAPI.Core.Models.UserEntities
{
    public class StaffInfo
    {   
        [Key]
        public int StaffInfo__Id { get; set; }
        public string? StaffInfo__FirstName { get; set; }
        public string? StaffInfo__LastName { get; set; }
        public string? StaffInfo__Phone { get; set; }
        [DefaultValue(HandleString.DefaultImage)]
        public string? StaffInfo__Avatar { get; set; }
        public int StaffInfo__AccountId { get; set; }
        [ForeignKey("StaffInfo__AccountId")]
        public IdentityAccount? Account { get; set; }
    }
}