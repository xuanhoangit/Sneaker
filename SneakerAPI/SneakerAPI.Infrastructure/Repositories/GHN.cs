// using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.GHN;
using SneakerAPI.Infrastructure.Data;

public class GHN:IGHN
{
    private readonly HttpClient _httpClient;
    private readonly SneakerAPIDbContext _context;
    private readonly string Token="070cb6e2-13c5-11f0-95d0-0a92b8726859";
    private readonly string ShopId="196342";
    public GHN(SneakerAPIDbContext context)
    {
        _httpClient = new HttpClient();
        _context = context;
    }
    public async Task<GHNResponse> GetOrderDetail(string order_code){
        var url="https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail";
        var requestBody = new
        {
            order_code = order_code
        };

        var json = System.Text.Json.JsonSerializer.Serialize(requestBody); // KHÔNG cần escape thủ côn
        var content = new StringContent(json, Encoding.UTF8, "application/json");
          // Thêm header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Token", Token);
         _httpClient.DefaultRequestHeaders.Add("ShopId", ShopId);
        // Gửi request
         var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        // Nếu HTTP 200 thì parse JSON
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<GHNResponse>(responseContent);
            Console.WriteLine($"Mã đơn hàng: {result.data.order_code}");
            return result;
        }
        return null;
    }
   public async Task<GHNResponse> CreateShippingOrderAsync(CreateOrderRequest request)
    {
        var url = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";
        // Tạo payload (request body)
         var payload = new
        {
            payment_type_id = 2,
            note = "Tintest 123",
            required_note = "KHONGCHOXEMHANG",
            // return_phone = "0332190158",
            // return_address = "39 NTT",
            return_district_id = (int?)null,
            return_ward_code = "",
            client_order_code = "",
            // from_name = "TinTest124",
            // from_phone = "0987654321",
            // from_address = "72 Thành Thái, Phường 14, Quận 10, Hồ Chí Minh, Vietnam",
            // from_ward_name = "Phường 14",
            // from_district_name = "Quận 10",
            // from_province_name = "HCM",
            to_name = "TinTest124",
            to_phone = "0987654321",
            to_address = "72 Thành Thái, Phường 10, Quận 10, Hồ Chí Minh, Vietnam",
           to_ward_code = "20308",
            to_district_id = 1444,
            to_province_name = "HCM",
            cod_amount = 200000,
            content = "Theo New York Times",
            length = 12,
            width = 12,
            height = 12,
            weight = 1200,
            // cod_failed_amount = 2000,
            // pick_station_id = 1444,
            // deliver_station_id = (int?)null,
            // insurance_value = 100000,
            service_type_id = 2,
            // coupon = (string)null,
            // pickup_time = 1692840132,
            pick_shift = new[] { 2 },
            // items = new[]
            // {
            //     new {
            //         name = "Áo Polo",
            //         code = "Polo123",
            //         quantity = 1,
            //         price = 200000,
            //         length = 12,
            //         width = 12,
            //         height = 12,
            //         weight = 1200,
            //         category = new {
            //             level1 = "Áo"
            //         }
            //     }
            // }
        };

        // Chuyển payload thành JSON
        var json = System.Text.Json.JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Thêm header
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Token", Token);
        _httpClient.DefaultRequestHeaders.Add("ShopId", ShopId);

        // Gửi request
        var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        // Nếu HTTP 200 thì parse JSON
    if (response.IsSuccessStatusCode)
    {
        var result = JsonConvert.DeserializeObject<GHNResponse>(responseContent);
        Console.WriteLine($"Mã đơn hàng: {result.data.order_code}");
        return result;
    }
    return null;
    }
}

