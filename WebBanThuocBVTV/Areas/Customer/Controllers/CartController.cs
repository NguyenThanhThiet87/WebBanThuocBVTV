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
            try
            {
                var accountJson = HttpContext.Session.GetString("Account");
                if (accountJson != null)
                {
                    Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);

                    Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);
                    if (gioHang.TongTien < 0)
                    {
                        double sum = 0;
                        foreach (var item in gioHang.GiohangSanphams)
                        {
                            sum += item.TongTien;
                        }
                        gioHang.TongTien = sum;
                        await _gioHangRepository.Update(gioHang);
                    }
                    return PartialView(gioHang);
                }
                return null;
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(string maSp, int soLuong)
        {
            try
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
                    gioHangSanPham.TongTien = (double)(sp.Gia * soLuong);
                    gioHang.TongTien += gioHangSanPham.TongTien;

                    AlertMessage alertSanPham = await _gioHangRepository.AddSanPham(gioHangSanPham);
                    if (alertSanPham.Type == "success")
                    {
                        AlertMessage alert = await _gioHangRepository.Update(gioHang);
                        return Json(new { success = true, message = alert.Message });
                    }
                    return Json(new { success = false, message = alertSanPham.Message });
                }
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveProduct(string maSp)
        {
            try
            {
                var accountJson = HttpContext.Session.GetString("Account");
                if (accountJson != null)
                {
                    Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                    Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);

                    GiohangSanpham gioHangSanPham = await _gioHangRepository.GetGioHangSanPham(gioHang.MaGioHang, maSp);
                    gioHang.TongTien -= gioHangSanPham.TongTien;
                    if (gioHang.TongTien < 0)
                        gioHang.TongTien = 0;
                    AlertMessage alertSanPham = await _gioHangRepository.Delete(gioHangSanPham);
                    if (alertSanPham.Type == "success")
                    {
                        AlertMessage alert = await _gioHangRepository.Update(gioHang);
                        return Json(new { success = true, message = alert.Message });
                    }
                    return Json(new { success = false, message = alertSanPham.Message });
                }
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSoluongProduct(string maSp, int soLuong)
        {
            try
            {
                var accountJson = HttpContext.Session.GetString("Account");
                if (accountJson != null)
                {
                    Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                    Giohang gioHang = await _gioHangRepository.GetById(account.MaNd);

                    GiohangSanpham gioHangSanPham = await _gioHangRepository.GetGioHangSanPham(gioHang.MaGioHang, maSp);
                    gioHang.TongTien -= gioHangSanPham.TongTien;
                    double gia = gioHangSanPham.TongTien / gioHangSanPham.SoLuong;
                    gioHangSanPham.SoLuong = soLuong;
                    gioHangSanPham.TongTien = gioHangSanPham.SoLuong * gia;
                    gioHang.TongTien += gioHangSanPham.TongTien;

                    AlertMessage alertSanPham = await _gioHangRepository.UpdateProduct(gioHangSanPham);
                    if (alertSanPham.Type == "success")
                    {
                        AlertMessage alert = await _gioHangRepository.Update(gioHang);
                        return Json(new { success = true, message = alert.Message });
                    }
                    return Json(new { success = false, message = alertSanPham.Message });
                }
                return Json(new { success = false, message = "Bạn chưa đăng nhập" });
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        
    }
}
