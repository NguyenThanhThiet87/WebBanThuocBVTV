using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using X.PagedList.Extensions;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : BaseController
    {
        private readonly NhomSanPhamRepository _nhomSanPhamRepository;
        private readonly SanPhamRepository _sanPhamRepository;
        private readonly BinhLuanRepository _binhLuanRepository;
        private readonly DanhGiaRepository _danhGiaRepository;
        private readonly NhaSanXuatRepository _nhaSanXuatRepository;
        public ProductController(SanPhamRepository sanPhamRepository, BinhLuanRepository binhLuanRepository, DanhGiaRepository danhGiaRepository, NhaSanXuatRepository nhaSanXuatRepository, NhomSanPhamRepository nhomSanPhamRepository)
        {
            _nhomSanPhamRepository = nhomSanPhamRepository;
            _sanPhamRepository = sanPhamRepository;
            _binhLuanRepository = binhLuanRepository;
            _danhGiaRepository = danhGiaRepository;
            _nhaSanXuatRepository = nhaSanXuatRepository;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                AddBreadcrum(new BreadcrumItem() { Text = "Sản Phẩm", Url = Url.Action("Index", "Product", new { area = "Customer" }) });//thêm vào breadcrum
                HttpContext.Session.SetString("IndexPage", "Product");//lưu vào session vị trí hiện tại của trang

                ViewBag.nhomSanPham = await _nhomSanPhamRepository.GetAllAsync();
                @ViewBag.maNhomSp = "P&H";
                ViewBag.NhaSx = await _nhaSanXuatRepository.GetAllAsync();

                return View();
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> FilterProduct(string maNhaSx, string maNhomSp, PriceArrange? priceArrange, QuantityOptions? quantityOption, SortPrice sortPrice, int? page)
        {
            try
            {
                maNhaSx = maNhaSx ?? "";
                maNhomSp = maNhomSp ?? "";
                if (page == null)
                    page = 1;

                List<Sanpham> lstProducts = await _sanPhamRepository.FilterProduct(maNhomSp: maNhomSp, maNhaSx: maNhaSx, sortPrice: sortPrice);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstProducts.Count / pageSize;

                return PartialView("_ListProduct", lstProducts.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        public async Task<IActionResult> DetailProduct(string maSp)
        {
            try
            {
                Sanpham sp = await _sanPhamRepository.GetById(maSp);

                AddBreadcrum(new BreadcrumItem() { Text = sp.TenSanPham, Url = Url.Action("DetailProduct", "Product", new { area = "Customer", maSp = maSp }) });//thêm vào breadcrum
                List<Danhgia> lstDanhGia = await _danhGiaRepository.GetAllAsync();
                ViewBag.lstDanhGia = lstDanhGia;
                return View(sp);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReviewingSanPham(string maSp, string content, string maDg)
        {
            try
            {
                var accountJson = HttpContext.Session.GetString("Account");
                if (accountJson != null)
                {
                    Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                    Binhluan binhluan = new Binhluan();
                    binhluan.MaSanPham = maSp;
                    binhluan.MaNd = account.MaNd;
                    binhluan.NoiDung = content;
                    binhluan.MaDanhGia = int.Parse(maDg);
                    binhluan.ThoiGian = DateTime.Now;

                    AlertMessage result = await _binhLuanRepository.Add(binhluan);
                    SetAlert(result.Message, result.Type);
                }
                else
                {
                    SetAlert("Bạn chưa đăng nhập tài khoản", "warning");
                }
                return RedirectToAction("DetailProduct", new { maSp });
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchProduct(string keyword)
        {
            try
            {
                List<Sanpham> lstSp = await _sanPhamRepository.FilterProduct(keyword);
                return PartialView("_SearchProduct", lstSp);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> FeaturedProduct(string keyword)
        {
            try
            {
                List<Sanpham> lstSp = await _sanPhamRepository.FeatureProduct();
                return PartialView("_FeatureProduct", lstSp);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SuggestProduct(string keyword)
        {
            try
            {
                List<Sanpham> lstSp = await _sanPhamRepository.SuggestProduct(keyword);
                return PartialView("_SuggestProduct", lstSp);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Detection");
            }
        }
    }
}
