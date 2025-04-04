using Microsoft.AspNetCore.Mvc;
using SneakerAPI.AdminApi.Controllers;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Models.Vnpay;
using SneakerAPI.Core.Utilities;
using SneakerAPI.Core.VnpayEnums;


[Route("api/Vnpay")]
public class VnpayPayment : BaseController
{

    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _uow;

    public VnpayPayment(IVnpay vnpay, IConfiguration configuration,IUnitOfWork uow):base(uow)
    {
        _configuration = configuration;
        _uow = uow;
        _uow.Vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:ReturnUrl"]);
    }
    [HttpGet("create-payment-url")]
    public ActionResult<string> CreatePaymentUrl(double moneyToPay, string description)
    {
        try
        {
            var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

            var request = new PaymentRequest
            {
                PaymentId = DateTime.Now.Ticks,
                Money = moneyToPay,
                Description = description,
                IpAddress = ipAddress,
                BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
            };

            var paymentUrl = _uow.Vnpay.GetPaymentUrl(request);

            return Created(paymentUrl, paymentUrl);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    // /IPN URL (backend) để thông báo trạng thái giao dịch.
    // Hệ thống backend nhận thông báo từ VNPAY thông qua IPN URL.

    [HttpGet("ipn")]
    public IActionResult IpnAction()
    {
        if (Request.QueryString.HasValue)
        {
            try
            {
                var paymentResult = _uow.Vnpay.GetPaymentResult(Request.Query);
                if (paymentResult.IsSuccess)
                {
                    // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                    var order=_uow.Order.FirstOrDefault(x=>x.Order__PaymentCode==paymentResult.PaymentId);
                    order.Order__Status=(int)OrderStatus.Completed;
                    _uow.Order.Update(order);
                    return Ok("Payment successful");
                }

                // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                return BadRequest("Payment failed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NotFound("Không tìm thấy thông tin thanh toán.");
    }
    //Callback URL (frontend) để điều hướng người dùng quay lại trang web.
    //Khi người dùng quay lại trang web qua Callback URL, hệ thống kiểm 
    // tra trạng thái giao dịch dựa trên thông tin VNPAY gửi kèm.
    // Hiển thị kết quả giao dịch (thành công/thất bại/lỗi) cho người dùng
    [HttpGet("callback")]
    public ActionResult<string> Callback()
    {
        if (Request.QueryString.HasValue)
        {
            try
            {
                var paymentResult = _uow.Vnpay.GetPaymentResult(Request.Query);
                var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                if (paymentResult.IsSuccess)
                {
                    return Ok(paymentResult);
                }

                return BadRequest(resultDescription);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NotFound("Information of payment is not valid.");
    }
}