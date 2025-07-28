using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using WebBanThuocBVTV.Helper;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            SavePointSideBar(SideBar.TongQuan);
            return View();
        }
    }
}
