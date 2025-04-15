namespace SneakerAPI.Core.Models.GHN;
public class FeeInfo
{
    public int main_service { get; set; }
    public int insurance { get; set; }
    public int station_do { get; set; }
    public int station_pu { get; set; }
    public int @return { get; set; }
    public int r2s { get; set; }
    public int coupon { get; set; }
    public int cod_failed_fee { get; set; }
}

public class OrderData
{
    public string order_code { get; set; }
    public string sort_code { get; set; }
    public string trans_type { get; set; }
    public string ward_encode { get; set; }
    public string district_encode { get; set; }
    public FeeInfo fee { get; set; }
    public string total_fee { get; set; }
    public DateTime expected_delivery_time { get; set; }
    public List<LogEntry> log { get; set; }
}
public class LogEntry
{
    public string status { get; set; }
    public string updated_date { get; set; }
    public string payment_type_id { get; set; }
}
public class GHNResponse
{
    public int code { get; set; }
    public string message { get; set; }
    public OrderData data { get; set; }
    public string message_display { get; set; }
}
