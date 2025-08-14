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
        readonly NguoiDungRepository _nguoiDungRepository;
        readonly SanPhamRepository _sanPhamRepository;
        readonly DonHangRepository _donHangRepository;

        public HomeController(ILogger<HomeController> logger, NguoiDungRepository nguoiDungRepository, SanPhamRepository sanPhamRepository, DonHangRepository donHangRepository)
        {
            _logger = logger;
            _nguoiDungRepository = nguoiDungRepository;
            _sanPhamRepository = sanPhamRepository;
            _donHangRepository = donHangRepository;
        }

        public async Task<IActionResult> Index()
        {
            SavePointSideBar(SideBar.TongQuan);
            Dictionary<string,int> staUser = _nguoiDungRepository.Statistic();
            ViewBag.statisticUser = staUser;
            Dictionary<string, int> staProduct = _sanPhamRepository.Statistic();
            ViewBag.statisticProduct = staProduct;
            Dictionary<string, int> staOrder = _donHangRepository.Statistic();
            ViewBag.statisticOrder = staOrder;
            ViewBag.CountProcessingOrder = await _donHangRepository.CountProcessingOrder();
            List<Sanpham> lstOutOsStock = await _sanPhamRepository.GetOutOfStockProduct();
            ViewBag.OutOfStock = lstOutOsStock;

            List<Donhang> dh = await _donHangRepository.GetNewOrders();

            return View(dh);
        }

        public async Task<List<KeyValuePair<DateTime, double>>> RevenueClostSixMonth()
        {
            Dictionary<DateTime, double> lst = await _donHangRepository.RevenueClostSixMonth();
            return lst.ToList();
        }
    }
}
