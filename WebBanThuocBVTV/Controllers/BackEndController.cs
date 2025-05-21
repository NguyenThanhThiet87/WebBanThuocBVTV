using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
namespace WebBanThuocBVTV.Controllers
{
    public class BackEndController : Controller
    {
        SanPhamRepository sanPhamRepository = new SanPhamRepository();
        NhomSanPhamRepository nhomSanPhamRepository = new NhomSanPhamRepository();
        NhaSanXuatRepository nhaSanXuatRepository = new NhaSanXuatRepository();
        HinhAnhRepository hinhAnhRepository = new HinhAnhRepository();
        public async Task<IActionResult> Index()
        {
            ViewBag.listnsp = await nhomSanPhamRepository.GetAllAsync();
            ViewBag.listnsx = await nhaSanXuatRepository.GetAllAsync();
            ViewBag.countsp = await sanPhamRepository.Count();
            return View("NhapLieu");
        }
        [HttpPost]
        public async Task<IActionResult> ThemSp(string UrlImg, string TenSanPham, string ThanhPhan, string CongDung, string HuongDanSuDung, decimal Gia, int SoLuong, string MaNhomSp, string MaNhaSx)
        {
            
            Sanpham sp = new Sanpham();
            sp.MaSanPham = await sanPhamRepository.CreateId();
            sp.TenSanPham = TenSanPham;
            sp.ThanhPhan = ThanhPhan;
            sp.CongDung = CongDung;
            sp.HuongDanSd = HuongDanSuDung;
            sp.Gia = Convert.ToDouble(Gia);
            sp.SoLuong = SoLuong;
            sp.MaNhomSp = MaNhomSp;
            sp.MaNhaSx = MaNhaSx;

            bool resultSp= await sanPhamRepository.Add(sp);
            if (resultSp)
            {
                Hinhanh img = new Hinhanh();
                img.MaHinhAnh = await hinhAnhRepository.CreateId();
                img.MaSanPham = sp.MaSanPham;
                img.Url = UrlImg;
                bool resultImg = await hinhAnhRepository.Add(img);
                ViewBag.SuccessImg = resultImg;
            }    

            ViewBag.Success = resultSp;
            
            return RedirectToAction("Index","BackEnd");
        }

    }


}
