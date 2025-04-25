using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace SneakerAPI.Core.Models.ProductEntities
{
    public class Brand
    {   
        [Key]
        public int Brand__Id { get; set; }
        [Required]
        public string? Brand__Name { get; set; }
        [Required]
        public string? Brand__Description { get; set; }
        [Required]
        public string? Brand__Logo { get; set; }
        public bool Brand__IsActive { get; set; }
        [NotMapped]
        public IFormFile? FileLogo { get; set; }
        public virtual List<Product>? Products { get; set; }
    }
}