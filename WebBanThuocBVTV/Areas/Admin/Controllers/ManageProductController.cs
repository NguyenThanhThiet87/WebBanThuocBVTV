using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using X.PagedList.Extensions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageProductController : BaseController
    {
        private Cloudinary_Net _cloudinary_Net;
        private SanPhamRepository _sanPhamRepository;
        private NhomSanPhamRepository _nhomSanPhamRepository;
        private NhaSanXuatRepository _nhaSanXuatRepository;
        public ManageProductController(SanPhamRepository sanPhamRepository, NhomSanPhamRepository nhomSanPhamRepository, NhaSanXuatRepository nhaSanXuatRepository, IConfiguration config)
        {
            _sanPhamRepository = sanPhamRepository;
            _nhomSanPhamRepository = nhomSanPhamRepository;
            _nhaSanXuatRepository = nhaSanXuatRepository;
            _cloudinary_Net = new Cloudinary_Net(config);
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                SavePointSideBar(SideBar.SanPham);

                List<Nhomsanpham> lstNSP = await _nhomSanPhamRepository.GetAllAsync();
                List<Nhasanxuat> lstNSX = await _nhaSanXuatRepository.GetAllAsync();
                ViewBag.lstNSP = lstNSP;
                ViewBag.lstNSX = lstNSX;
                return View();
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DetailProduct(string maSp)
        {
            try
            {
                Sanpham sp = await _sanPhamRepository.GetById(maSp);
                List<Nhomsanpham> lstNSP = await _nhomSanPhamRepository.GetAllAsync();
                List<Nhasanxuat> lstNSX = await _nhaSanXuatRepository.GetAllAsync();
                ViewBag.lstNSP = lstNSP;
                ViewBag.lstNSX = lstNSX;
                return PartialView("_DetailProduct", sp);
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchProduct(string keyword, int? page)
        {
            try
            {
                if (keyword == null)
                    keyword = "";
                if (page == null)
                    page = 1;

                List<Sanpham> lstProducts = await _sanPhamRepository.FilterProduct(keyword);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstProducts.Count / pageSize;

                return PartialView("_ListProduct", lstProducts.ToPagedList(pageNumber, pageSize));
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> UpdateProduct(Sanpham sp)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                alertMessage = await _sanPhamRepository.Update(sp);
                if (alertMessage.Type.ToString() == "success")
                {
                    SetAlert(alertMessage.Message, "success");
                }
                else
                {
                    SetAlert($"Cập nhật thất bại: {alertMessage.Message}", "error");
                }
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public async Task<IActionResult> AddProductPartialView()
        {
            try
            {
                string maSp = await _sanPhamRepository.CreateId();
                List<Nhomsanpham> lstNSP = await _nhomSanPhamRepository.GetAllAsync();
                List<Nhasanxuat> lstNSX = await _nhaSanXuatRepository.GetAllAsync();
                ViewBag.lstNSP = lstNSP;
                ViewBag.lstNSX = lstNSX;
                ViewBag.maSp = maSp;
                return PartialView("_AddProduct");
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<AlertMessage> AddProduct(Sanpham sp, IFormFile imgProduct)
        {
            try
            {
                string url = _cloudinary_Net.Upload(imgProduct, sp.MaNhomSp);
                if (String.IsNullOrEmpty(url))
                {
                    AlertMessage alertMessageERR = new AlertMessage();
                    alertMessageERR.Type = "error";
                    alertMessageERR.Message = "Xảy ra lỗi trên cloudinary";
                    return alertMessageERR;
                }
                Hinhanh img = new Hinhanh();
                img.MaSanPham = sp.MaSanPham;
                img.Url = url;
                sp.Hinhanhs.Add(img);

                AlertMessage alertMessage = await _sanPhamRepository.Add(sp);
                return alertMessage;
            }catch(Exception ex)
            {
                return new AlertMessage
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterProduct(string keyword,int isActive, string maNhaSx, string maNhomSp, PriceArrange? priceArrange, QuantityOptions? quantityOption, SortOptions sortOption, int? page)
        {
            try
            {
                bool isActiveProduct = isActive == 1 ? false : true;
                keyword = keyword ?? "";
                maNhaSx = maNhaSx ?? "";
                maNhomSp = maNhomSp ?? "";
                if (page == null)
                    page = 1;

                List<Sanpham> lstProducts = await _sanPhamRepository.FilterProduct(keyword, isActiveProduct, maNhomSp, maNhaSx, priceArrange, quantityOption, sortOption);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstProducts.Count / pageSize;

                return PartialView("_ListProduct", lstProducts.ToPagedList(pageNumber, pageSize));
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> DiscontinueProduct(string maSp)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                alertMessage = await _sanPhamRepository.Discontinue(maSp);
            }catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }
        [HttpPost]
        public async Task<AlertMessage> SellProduct(string maSp)
        {
            try
            {
                AlertMessage alertMessage = await _sanPhamRepository.Sell(maSp);
                return alertMessage;
            }
            catch (Exception ex)
            {
                return new AlertMessage
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }
    }
}
