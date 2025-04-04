namespace SneakerAPI.Core.Models.Filters
{   
    public enum Sort{
        Default=0,
        DateDecrease,
        DateIncrease,
        ZA,
        AZ,
        PriceDecrease,
        PriceIncrease
    }
    public class RangeDateTime{
        public DateTime? From {get;set;}
        public DateTime? To {get;set;} 
    }
    public class OrderFilter
    {
        public int? Order__Status {get;set;}
        public int? Account__Id { get; set; }
        public int? Payment__Status {get;set;}
        public RangePrice? RangePrice {get;set;}
        public RangeDateTime? RangeDateTime {get;set;}
        public int SortBy { get; set; }= (int)Sort.Default;
    }
}