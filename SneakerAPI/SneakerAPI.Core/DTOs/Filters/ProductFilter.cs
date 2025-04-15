namespace SneakerAPI.Core.Models.Filters
{   
    public class RangePrice{
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
    public enum SaleMaketting{
        BestSeller,
        NewArrival
    }

    public class ProductFilter
    {   
        
        public string? SearchString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? FromDate { get; set; }
        public int[]? ColorIds { get; set; }
        public List<int>? BrandIds { get; set; }
        public List<int>? SizeIds{ get; set; }
        public RangePrice? RangePrice { get; set; }
        public bool OrderByDatetime {get;set;}
    }
}