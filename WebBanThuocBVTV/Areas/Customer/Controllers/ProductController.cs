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
        private NhomSanPhamRepository nhomSanPhamRepository = new NhomSanPhamRepository();
        private SanPhamRepository _sanPhamRepository;
        private BinhLuanRepository _binhLuanRepository;
        private DanhGiaRepository _danhGiaRepository;
        public ProductController(SanPhamRepository sanPhamRepository, BinhLuanRepository binhLuanRepository, DanhGiaRepository danhGiaRepository)
        {
            _sanPhamRepository = sanPhamRepository;
            _binhLuanRepository = binhLuanRepository;
            _danhGiaRepository = danhGiaRepository;
        }
        public async Task<IActionResult> Index(int? page, string maNhomSp )
        {
            AddBreadcrum(new BreadcrumItem() { Text="Sản Phẩm", Url= Url.Action("Index","Product",new {area = "Customer"})});//thêm vào breadcrum
            HttpContext.Session.SetString("IndexPage", "Product");//lưu vào session vị trí hiện tại của trang

            if (page == null)
                page = 1;

            if (string.IsNullOrEmpty(maNhomSp))
                maNhomSp = "P&H";

            ViewBag.nhomSanPham = await nhomSanPhamRepository.GetAllAsync();
            List<Sanpham> lstSanPham = null;
            lstSanPham = await _sanPhamRepository.FilterProduct(maNhomSp);

            int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

            int pageNumber = page ?? 1;


            ViewBag.maNhomSp = maNhomSp;
            return View(lstSanPham.ToPagedList(pageNumber, pageSize));
        }
        [HttpPost]
        public async Task<IActionResult> FilterProduct(string maNhomSp)
        {
            return RedirectToAction("Index",new { maNhomSp});
        }
        public async Task<IActionResult> DetailProduct(string maSp)
        {
            AddBreadcrum(new BreadcrumItem() { Text = maSp, Url = Url.Action("DetailProduct","Product",new {area = "Customer", maSp = maSp}) });//thêm vào breadcrum

            Sanpham sp = await _sanPhamRepository.GetById(maSp);
            List<Danhgia> lstDanhGia = await _danhGiaRepository.GetAllAsync();
            ViewBag.lstDanhGia = lstDanhGia;
            return View(sp);
        }
        [HttpPost]
        public async Task<IActionResult> ReviewingSanPham(string maSp, string content, string maDg)
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
            return RedirectToAction("DetailProduct", new { maSp });
        }

        [HttpPost]
        public async Task<IActionResult> SearchProduct(string keyword)
        {
            List<Sanpham> lstSp = await _sanPhamRepository.FilterProduct(keyword);
            return PartialView("_SearchProduct", lstSp);
        }

        public async Task<IActionResult> FeaturedProduct(string keyword)
        {
            List<Sanpham> lstSp = await _sanPhamRepository.FeatureProduct();
            return PartialView("_FeatureProduct", lstSp);
        }
    }
}
