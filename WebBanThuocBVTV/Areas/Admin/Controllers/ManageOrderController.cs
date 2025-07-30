using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using WebBanThuocBVTV.Repositories.Interfaces;
using X.PagedList.Extensions;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageOrderController : BaseController
    {
        readonly DonHangRepository _donHangRepository;
        readonly TrangThaiRepository _trangThaiRepository;

        public ManageOrderController(DonHangRepository donHangRepository, TrangThaiRepository trangThaiRepository)
        {
            _donHangRepository = donHangRepository;
            _trangThaiRepository = trangThaiRepository;
        }

        public async Task<IActionResult> Index()
        {
            SavePointSideBar(SideBar.DonHang);

            ViewBag.TrangThaiDonHang = await _trangThaiRepository.GetAllAsync();
            return View();
        }
        [HttpPost]
        public IActionResult DetailOrder(string maDh)
        {
            Donhang dh = _donHangRepository.Get(maDh);

            return PartialView("_DetailOrder", dh);
        }

        [HttpPost]
        public async Task<IActionResult> FilterOrder(string id, string state, SortOptionsOrder sortOption, int? page)
        {
            id = id ?? "";
            if (page == null)
                page = 1;

            state = state ?? "";
            List<Donhang> lstCustomers = await _donHangRepository.FilterOrder(id, state, sortOption);

            int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

            int pageNumber = page ?? 1;

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = lstCustomers.Count / pageSize;

            return PartialView("_ListOrder", lstCustomers.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public async Task<AlertMessage> SendOrder(string maDh)
        {
            AlertMessage alertMessage = await _donHangRepository.SendOrder(maDh);
            return alertMessage;
        }
        [HttpPost]
        public async Task<AlertMessage> TransferredOrder(string maDh)
        {
            AlertMessage alertMessage = await _donHangRepository.TransferredOrder(maDh);
            return alertMessage;
        }
    }
}
