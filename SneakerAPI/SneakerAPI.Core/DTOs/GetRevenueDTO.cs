namespace SneakerAPI.Core.DTOs
{
    public class GetRevenueByDayDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
    }
    public class GetRevenueByMonthDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Revenue { get; set; }
    }
}