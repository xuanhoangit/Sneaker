namespace SneakerAPI.Core.Models.Filters
{   
    public class RangePrice{
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
    public class ProductFilter
    {   
        public string? SearchString { get; set; }
        public int[]? ColorIds { get; set; }
        public List<int>? BrandIds { get; set; }
        public List<string>? SizeNames{ get; set; }
        public RangePrice? RangePrice { get; set; }
    }
}