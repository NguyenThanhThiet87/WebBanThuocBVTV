using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Models.VnPay;
using WebBanThuocBVTV.Repositories;


namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : BaseController
    {
        private NguoiDungRepository _nguoiDungRepository;
        private SanPhamRepository _sanPhamRepository;
        private DonHangRepository _donHangRepository;

        public OrderController(NguoiDungRepository nguoiDungRepository, SanPhamRepository sanPhamRepository, DonHangRepository donHangRepository)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _sanPhamRepository = sanPhamRepository;
            _donHangRepository = donHangRepository;
        }
        [HttpPost]
        public async Task<IActionResult> Index(string maSp, int soLuong, string listSanPhamOrderStr, int status) //status 0 là mua ngay, 1 là mua giỏ hàng
        {
            
            List<DonhangSanpham> listSanPhamOrder = listSanPhamOrderStr!=null? JsonSerializer.Deserialize<List<DonhangSanpham>>(listSanPhamOrderStr): null;

            AlertMessage alertMessage = new AlertMessage();

            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                Nguoidung user = await _nguoiDungRepository.GetById(account.MaNd);
                ViewBag.User = user;

                if (string.IsNullOrEmpty(user.SoDienThoai) || string.IsNullOrEmpty(user.DiaChi))
                {
                    if (string.IsNullOrEmpty(user.SoDienThoai))
                    {
                        alertMessage.Type = "warning";
                        alertMessage.Message = "Bạn chưa xác thực số điện thoại! Vui lòng xác thực số điện thoại trước khi mua hàng.";
                    }
                    if (string.IsNullOrEmpty(user.DiaChi))
                    {
                        alertMessage.Type = "warning";
                        alertMessage.Message = "Bạn chưa cập nhật địa chỉ giao hàng! Vui lòng cập nhật địa chỉ trước khi mua hàng.";
                    }
                }
                else
                {
                    if (status == 0)
                    {
                        Sanpham sp = await _sanPhamRepository.GetById(maSp);
                        DonhangSanpham donHangSanPham = new DonhangSanpham();
                        donHangSanPham.MaDonHang = _donHangRepository.CreateId();
                        donHangSanPham.MaSanPham = maSp;
                        donHangSanPham.SoLuongDatMua = soLuong;
                        donHangSanPham.MaSanPhamNavigation = sp;
                        donHangSanPham.TongTien = soLuong * donHangSanPham.MaSanPhamNavigation.Gia;

                        listSanPhamOrder = new List<DonhangSanpham>();
                        listSanPhamOrder.Add(donHangSanPham);
                    }else
                    {
                        string maDonHang = _donHangRepository.CreateId();
                        foreach (var dhsp in listSanPhamOrder)
                        {
                            Sanpham sp = await _sanPhamRepository.GetById(dhsp.MaSanPham);

                            dhsp.MaDonHang = maDonHang;
                            dhsp.MaSanPhamNavigation = sp;
                            dhsp.TongTien = dhsp.SoLuongDatMua * dhsp.MaSanPhamNavigation.Gia;
                        }
                    }    

                    return View(listSanPhamOrder);
                }
            }
            else
            {
                alertMessage.Type = "warning";
                alertMessage.Message = "Bạn chưa đăng nhập";
            }
            SetAlert(alertMessage.Message, alertMessage.Type);
            if (status == 0)
                return RedirectToAction("DetailProduct", "Product", new { maSp });
            else
                return  Json(new {success = alertMessage.Type, message = alertMessage.Message });
        }
        [HttpPost]
        public async Task<IActionResult> PaymentAsync(List<DonhangSanpham> orderItems, string ghiChu, string pttt)
        {
            var accountJson = HttpContext.Session.GetString("Account");
            if (accountJson != null)
            {
                
                Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(accountJson);
                Donhang donhang = new Donhang();
                donhang.MaDonHang = _donHangRepository.CreateId();
                donhang.NgayLap = DateTime.Now;
                donhang.MaNd = account.MaNd;
                donhang.MaPhuongThucTt = pttt;
                donhang.GhiChu = ghiChu;
                donhang.TongTien = orderItems.Sum(dh => dh.TongTien);
                donhang.MaTrangThai = pttt=="NH"?"DXL":"CTT";

                foreach (DonhangSanpham sp in orderItems)
                {
                    sp.MaDonHang = donhang.MaDonHang;
                }

                AlertMessage result = await _donHangRepository.Add(orderItems, donhang);

                if (pttt == "VP")
                {
                    PaymentInformationModel paymentInformationModel = new PaymentInformationModel();
                    paymentInformationModel.OrderId = donhang.MaDonHang;
                    paymentInformationModel.OrderType = "270000";
                    paymentInformationModel.Amount = donhang.TongTien;
                    paymentInformationModel.Name = account.HoTen;
                    paymentInformationModel.OrderDescription = $"thanh toán VNPAY";
                    
                    return RedirectToAction("CreatePaymentUrlVnpay", "VnPay", paymentInformationModel);
                }
                
                SetAlert(result.Message, result.Type);
                return RedirectToAction("DetailOrder", "Order", new { donhang.MaDonHang });  
            }
            return RedirectToAction("Index");
        }
   
        public async Task<IActionResult> DetailOrder(string maDonHang)
        {
            AddBreadcrum(new BreadcrumItem() { Text = maDonHang, Url = Url.Action("DetailOrder","Order",new {area = "Customer", maDonHang=maDonHang}) });//thêm vào breadcrum

            Donhang donHang = _donHangRepository.Get(maDonHang);
            if (donHang != null)
            {
                return View(donHang);
            }
            SetAlert("Đơn hàng không tồn tại", "warning");
            return RedirectToAction("Index", "Home");
        }
    }
}
