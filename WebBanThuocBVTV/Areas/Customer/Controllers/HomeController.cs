using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WebBanThuocBVTV.Helper;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : BaseController
    {
        SendOTP _sendOTP;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration _config)
        {
            _sendOTP = new SendOTP(_config);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                AddBreadcrum(new BreadcrumItem() { Text = "Trang Chủ", Url = Url.Action("Index", "Home", new { area = "Customer" }) });
                Nguoidung Account = new Nguoidung();
                //_sendOTP.SendOTPByPhone("");
                //_sendOTP.CheckOTPByPhone("+16464066829", "328457");
                HttpContext.Session.SetString("IndexPage", "Home");//lưu vào session vị trí hiện tại của trang

                return View();
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
