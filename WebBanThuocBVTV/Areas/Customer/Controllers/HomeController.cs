using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            AddBreadcrum(new BreadcrumItem() { Text = "Trang Chủ", Url = Url.Action("Index", "Home", new { area = "Customer" }) });
            Nguoidung Account = new Nguoidung();

            HttpContext.Session.SetString("IndexPage", "Home");//lưu vào session vị trí hiện tại của trang

            return View();
        }
    }
}
