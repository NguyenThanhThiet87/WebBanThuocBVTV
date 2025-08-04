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
        readonly NguoiDungRepository _nguoiDungRepository;
        readonly SanPhamRepository _sanPhamRepository;

        public ManageOrderController(DonHangRepository donHangRepository, TrangThaiRepository trangThaiRepository, NguoiDungRepository nguoiDungRepository, SanPhamRepository sanPhamRepository)
        {
            _donHangRepository = donHangRepository;
            _trangThaiRepository = trangThaiRepository;
            _nguoiDungRepository = nguoiDungRepository;
            _sanPhamRepository = sanPhamRepository;
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
        [HttpPost]
        public async Task<AlertMessage> DeleteOrder(string maDh)
        {
            AlertMessage alertMessage = await _donHangRepository.DeleteOrder(maDh);
            return alertMessage;
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderPartialView()
        {

            return PartialView("_AddOrder");
        }
        public async Task<IActionResult> GetInfoCustomer(string maNd)
        {
            var nguoiDung = await _nguoiDungRepository.GetById(maNd);
            if(nguoiDung!=null)
            {
                return Json(new { success = true, data = nguoiDung });
            }    
            return Json(new { success = false, data = nguoiDung });
        }
        public async Task<IActionResult> GetInfoProduct(string maSp)
        {
            var sanPham = await _sanPhamRepository.GetByIdBase(maSp);
            if (sanPham != null)
            {
                return Json(new { success = true, data = sanPham });
            }
            return Json(new { success = false, data = sanPham });
        }
        [HttpPost]
        public async Task<AlertMessage> AddOrder(Donhang dh)
        {
            dh.MaDonHang = _donHangRepository.CreateId();
            dh.NgayLap = DateTime.Now;
            dh.MaTrangThai = "PCD";
            double sumPrice = 0;
            foreach(var dhsp in dh.DonhangSanphams)
            {
                dhsp.MaDonHang = dh.MaDonHang;
                sumPrice += dh.TongTien;
            }
            dh.TongTien = sumPrice;
            AlertMessage alertMessage = await _donHangRepository.Add(dh.DonhangSanphams.ToList(), dh);
            return alertMessage;
        }
        [HttpPost]
        public async Task<AlertMessage> AddOrderGuest(Donhang dh, Nguoidung nd)
        {
            nd.MaNd = await _nguoiDungRepository.CreateId();
            AlertMessage alertCreatUser = await _nguoiDungRepository.AddGuest(nd);
            if(alertCreatUser.Type=="success")
            {
                dh.MaDonHang = _donHangRepository.CreateId();
                dh.MaNd = nd.MaNd;
                dh.NgayLap = DateTime.Now;
                dh.MaTrangThai = "PCD";
                double sumPrice = 0;
                foreach (var dhsp in dh.DonhangSanphams)
                {
                    dhsp.MaDonHang = dh.MaDonHang;
                    sumPrice += dh.TongTien;
                }
                dh.TongTien = sumPrice;
                AlertMessage alertMessage = await _donHangRepository.Add(dh.DonhangSanphams.ToList(), dh);
                return alertMessage;
            }else if(alertCreatUser.Type=="exist")
            {
                nd.MaNd = alertCreatUser.Message;
                dh.MaDonHang = _donHangRepository.CreateId();
                dh.MaNd = nd.MaNd;
                dh.NgayLap = DateTime.Now;
                dh.MaTrangThai = "PCD";
                double sumPrice = 0;
                foreach (var dhsp in dh.DonhangSanphams)
                {
                    dhsp.MaDonHang = dh.MaDonHang;
                    sumPrice += dh.TongTien;
                }
                dh.TongTien = sumPrice;
                AlertMessage alertMessage = await _donHangRepository.Add(dh.DonhangSanphams.ToList(), dh);
                return alertMessage;
            }    
            else
            {
                return alertCreatUser;
            }
        }
    }
}
