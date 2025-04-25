using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Api.Controllers;
using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Interfaces;

using Microsoft.Extensions.Logging;
using SneakerAPI.Core.Utilities;
using SneakerAPI.Core.Models.Vnpay;
using SneakerAPI.Core.VnpayEnums;
using SneakerAPI.AdminApi.Controllers;

[Route("api/payment")]
[ApiController]
// [ApiExplorerSettings(IgnoreApi =true)]
public class VnpayPayment : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<VnpayPayment> _logger;

    public VnpayPayment( IConfiguration configuration, IUnitOfWork uow, ILogger<VnpayPayment> logger) : base(uow)
    {
        _configuration = configuration;
        _uow = uow;
        _logger = logger;

        _uow.Vnpay.Initialize(
            _configuration["Vnpay:TmnCode"],
            _configuration["Vnpay:HashSecret"],
            _configuration["Vnpay:BaseUrl"],
            _configuration["Vnpay:ReturnUrl"]
        );
    }

    [HttpGet("create-payment-url")]
    public ActionResult<string> CreatePaymentUrl(double moneyToPay, long orderPayment,string command, string description)
    {
        try
        {
            if (moneyToPay <= 0)
                return BadRequest("Invalid payment amount.");

            if (orderPayment <= 0)
                return BadRequest("Invalid order ID.");

            var ipAddress = NetworkHelper.GetIpAddress(HttpContext);

            var request = new PaymentRequest
            {
                PaymentId = orderPayment,
                Money = moneyToPay,
                Description = description,
                IpAddress = ipAddress,
                Command=command,
                BankCode = BankCode.ANY,
                CreatedDate = DateTime.Now,
                Currency = Currency.VND,
                Language = DisplayLanguage.Vietnamese
            };

            var paymentUrl = _uow.Vnpay.GetPaymentUrl(request);
           
            return Created(paymentUrl, paymentUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating payment URL.");
            return StatusCode(500, "Internal server error.");
        }
    }

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
                    if(paymentResult.Command==CommandType.Refund.ToString().ToLower())
                    {
                          var orderRefund=_uow.Order.FirstOrDefault(x=>x.Order__PaymentCode==paymentResult.PaymentId);
                            orderRefund.Order__PaymentStatus=(int)PaymentStatus.Refunded;
                            var refundResult=_uow.Order.Update(orderRefund);
                            return Ok(new {refundResult, message="Order refunded"});
                    }
                    // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                    var orderPay=_uow.Order.FirstOrDefault(x=>x.Order__PaymentCode==paymentResult.PaymentId);
                    orderPay.Order__PaymentStatus=(int)PaymentStatus.Paid;
                    var resultPay=_uow.Order.Update(orderPay);
                    return Ok(new {resultPay,message="Order payment successfully"});
                }

                // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                return BadRequest("Thanh toán thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        return NotFound("Không tìm thấy thông tin thanh toán.");
    }
    [HttpGet("callback")]
    public ActionResult<string> Callback()
    {
        if (!Request.QueryString.HasValue)
            return BadRequest("Invalid payment information.");

        try
        {
            var paymentResult = _uow.Vnpay.GetPaymentResult(Request.Query);
            _logger.LogInformation("Callback received: {@paymentResult}", paymentResult);

            var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

            return paymentResult.IsSuccess ? Ok(paymentResult) : BadRequest(resultDescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing callback.");
            return StatusCode(500, "Internal server error.");
        }
    }
}
