using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Twilio.Rest.Trunking.V1;
using Twilio.TwiML.Voice;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : BaseController
    {
        GioHangRepository _gioHangRepository;
        SanPhamRepository _sanPhamRepository;
        public CartController(GioHangRepository gioHangRepository, SanPhamRepository sanPhamRepository)
        {
            _gioHangRepository = gioHangRepository;
            _sanPhamRepository = sanPhamRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);

                Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);
                return PartialView(gioHang);
            }
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string maSp, int soLuong)
        {
            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);
                Sanpham sp = await _sanPhamRepository.GetById(maSp);
                GiohangSanpham gioHangSanPham = new GiohangSanpham();
                gioHangSanPham.MaGioHang = gioHang.MaGioHang;
                gioHangSanPham.MaSanPham = maSp;
                gioHangSanPham.SoLuong = soLuong;
                gioHangSanPham.TongTien = soLuong * sp.Gia;

                AlertMessage alert = await _gioHangRepository.AddSanPham(gioHangSanPham);
                return Json(new { success = true, message = alert.Message});
            }
            return Json(new { success = false, message = "Bạn chưa đăng nhập" });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveProduct(string maSp)
        {
            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);

                GiohangSanpham gioHangSanPham = await _gioHangRepository.GetGioHangSanPham(gioHang.MaGioHang, maSp);

                AlertMessage alert = await _gioHangRepository.Delete(gioHangSanPham);

                return Json(new { success = true, message = alert.Message });
            }
            return Json(new { success = false, message = "Bạn chưa đăng nhập" });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSoluongProduct(string maSp, int soLuong)
        {
            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);

                GiohangSanpham gioHangSanPham = await _gioHangRepository.GetGioHangSanPham(gioHang.MaGioHang, maSp);
                double gia = gioHangSanPham.TongTien / gioHangSanPham.SoLuong;
                gioHangSanPham.SoLuong = soLuong;
                gioHangSanPham.TongTien = gioHangSanPham.SoLuong * gia;

                AlertMessage alert = await _gioHangRepository.UpdateProduct(gioHangSanPham);

                return Json(new { success = true, message = alert.Message });
            }
            return Json(new { success = false, message = "Bạn chưa đăng nhập" });
        }
        
    }
}
