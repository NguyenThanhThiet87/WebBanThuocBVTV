using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using WebBanThuocBVTV.Helper.VnPay;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Models.VnPay;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class VnPayController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly WebBanThuocBvtvContext _contextDB;
        public VnPayController(IVnPayService vnPayService, WebBanThuocBvtvContext contextDB)
        {
            _vnPayService = vnPayService;
            _contextDB = contextDB;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            try
            {
                var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

                return Redirect(url);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                PaymentResponseModel response = _vnPayService.PaymentExecute(Request.Query);
                Donhang donHang = await _contextDB.Donhangs.Where(dh => dh.MaDonHang == response.OrderId).FirstOrDefaultAsync();
                if (donHang != null && response.VnPayResponseCode == "00")
                {
                    ViewBag.DonHang = donHang;
                    donHang.MaTrangThai = "PCD";
                    donHang.MaPhuongThucTt = "VP";
                    _contextDB.Donhangs.Update(donHang);

                    Giaodich giaoDich = new Giaodich();
                    giaoDich.MaGiaoDich = response.TransactionId;
                    giaoDich.MaDonHang = donHang.MaDonHang;
                    giaoDich.MaNganHang = response.BankCode;
                    giaoDich.TongTien = donHang.TongTien;
                    giaoDich.NoiDung = response.OrderDescription;
                    giaoDich.ThoiGian = DateTime.Now;

                    await _contextDB.Giaodiches.AddAsync(giaoDich);
                    await _contextDB.SaveChangesAsync();
                    SetAlert("Thanh toán thành công", "success");
                }
                else
                {
                    string message = "";
                    switch (response.VnPayResponseCode)
                    {
                        case "09":
                            message = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng";
                            break;
                        case "10":
                            message = "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
                            break;
                        case "11":
                            message = "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.";
                            break;
                        case "12":
                            message = "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.";
                            break;
                        case "13":
                            message = "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.";
                            break;
                        case "24":
                            message = "Giao dịch không thành công do: Khách hàng hủy giao dịch";
                            break;
                        case "51":
                            message = "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.";
                            break;
                        case "65":
                            message = "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.";
                            break;
                        case "75":
                            message = "Ngân hàng thanh toán đang bảo trì.";
                            break;
                        case "79":
                            message = "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch";
                            break;
                        default:
                            message = "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)";
                            break;
                    }

                    SetAlert(message, "warning");
                    return RedirectToAction("DetailOrder", "Order", new { donHang.MaDonHang });
                }
                return View("PaymentCallbackVnpay", response);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
