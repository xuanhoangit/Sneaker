namespace SneakerAPI.Core.Models.GHN;
public class Shipping
{
    public int OrderId { get; set; }
    public string RequiredNote { get; set; }
    public int ServiceTypeId { get; set; }
    public int PaymentTypeId { get; set; }
}