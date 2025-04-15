using System.Text.Json.Serialization;

public class CreateOrderRequest
{
    [JsonPropertyName("deliver_station_id")]
    public int? DeliverStationId { get; set; } = null;
    [JsonPropertyName("from_name")]
    public string FromName { get; set; } = string.Empty;

    [JsonPropertyName("from_phone")]
    public string FromPhone { get; set; } = string.Empty;

    [JsonPropertyName("from_address")]
    public string FromAddress { get; set; } = string.Empty;

    [JsonPropertyName("from_ward_code")]
    public string FromWardCode { get; set; } = string.Empty;

    [JsonPropertyName("from_district_id")]
    public int FromDistrictId { get; set; } 

    [JsonPropertyName("from_province_name")]
    public string FromProvinceName { get; set; } = string.Empty;

    [JsonPropertyName("to_name")]
    public string ToName { get; set; } = string.Empty;

    [JsonPropertyName("to_phone")]
    public string ToPhone { get; set; } = string.Empty;

    [JsonPropertyName("to_address")]
    public string ToAddress { get; set; } = string.Empty;

    [JsonPropertyName("to_ward_code")]
    public string ToWardCode { get; set; } = string.Empty;

    [JsonPropertyName("to_district_id")]
    public int ToDistrictId { get; set; } 

    [JsonPropertyName("to_province_name")]
    public string ToProvinceName { get; set; } = string.Empty;

    [JsonPropertyName("return_phone")]
    public string ReturnPhone { get; set; } = string.Empty;

    [JsonPropertyName("return_address")]
    public string ReturnAddress { get; set; } = string.Empty;

    [JsonPropertyName("return_district_id")]
    public int? ReturnDistrictId { get; set; } 

    [JsonPropertyName("return_ward_code")]
    public string? ReturnWardCode { get; set; } = string.Empty;

    [JsonPropertyName("return_province_name")]
    public string ReturnProvinceName { get; set; } = string.Empty;

    [JsonPropertyName("client_order_code")]
    public string ClientOrderCode { get; set; } = string.Empty;

    [JsonPropertyName("cod_amount")]
    public int CodAmount { get; set; } = 0;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("weight")]
    public int? Weight { get; set; } = 0;

    [JsonPropertyName("length")]
    public int? Length { get; set; } = 0;

    [JsonPropertyName("width")]
    public int? Width { get; set; } = 0;

    [JsonPropertyName("height")]
    public int? Height { get; set; } = 0;

    [JsonPropertyName("pick_station_id")]
    public int? PickStationId { get; set; } = 0;

    [JsonPropertyName("insurance_value")]
    public int InsuranceValue { get; set; } = 0;

    [JsonPropertyName("coupon")]
    public string Coupon { get; set; } = string.Empty;

    [JsonPropertyName("service_type_id")]
    public int ServiceTypeId { get; set; } = 0;
    [JsonPropertyName("service_id")]
    public int ServiceId { get; set; } = 0;

    [JsonPropertyName("payment_type_id")]
    public int PaymentTypeId { get; set; } = 0;

    [JsonPropertyName("note")]
    public string Note { get; set; } = string.Empty;

    [JsonPropertyName("required_note")]
    public string RequiredNote { get; set; } = string.Empty;

    [JsonPropertyName("pick_shift")]
    public List<int> PickShift { get; set; } = new();

    [JsonPropertyName("pickup_time")]
    public int? PickupTime { get; set; } = 0;

    [JsonPropertyName("items")]
    public List<ItemDto> Items { get; set; } = new();

    [JsonPropertyName("cod_failed_amount")]
    public int? CodFailedAmount { get; set; } = 0;
}
public class ItemDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 0;

    [JsonPropertyName("price")]
    public int Price { get; set; } = 0;

    [JsonPropertyName("length")]
    public int? Length { get; set; } = 0;

    [JsonPropertyName("width")]
    public int? Width { get; set; } = 0;

    [JsonPropertyName("weight")]
    public int? Weight { get; set; } = 0;

    [JsonPropertyName("height")]
    public int? Height { get; set; } = 0;

    [JsonPropertyName("category")]
    public ItemCategoryDto Category { get; set; } = new();
}
public class ItemCategoryDto
{
    [JsonPropertyName("level1")]
    public string Level1 { get; set; } = string.Empty;

    [JsonPropertyName("level2")]
    public string Level2 { get; set; } = string.Empty;

    [JsonPropertyName("level3")]
    public string Level3 { get; set; } = string.Empty;
}
