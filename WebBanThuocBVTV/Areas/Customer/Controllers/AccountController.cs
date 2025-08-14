using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;


namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : BaseController
    {
        private readonly SendOTP _sendOTP;
        private NguoiDungRepository _nguoiDungRepository;
        private DonHangRepository _donHangRepository;
        private Cloudinary_Net _cloudinary_Net;
        public AccountController(SendOTP sendOTP, NguoiDungRepository nguoiDungRepository, DonHangRepository donHangRepository, IConfiguration _config)
        {
            _sendOTP = sendOTP;
            _nguoiDungRepository = nguoiDungRepository;
            _donHangRepository = donHangRepository;
            _cloudinary_Net = new Cloudinary_Net(_config);
        }

        public async Task<IActionResult> Index()
        {
            AddBreadcrum(new BreadcrumItem() { Text = "Tài Khoản", Url = Url.Action("Index", "Account", new { area = "Customer" }) });//thêm vào breadcrum

            Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session?.GetString("Account"));
            if (account == null)
            {
                SetAlert("Bạn chưa đăng nhập", "warning");
                return RedirectToAction("Index", "Home");
            }
            Nguoidung user = await _nguoiDungRepository.GetById(account.MaNd);
            ViewBag.User = user;

            //Lấy thông tin lịch sử đơn hàng đã đặt
            List<Donhang> lstDonHangSp = await _donHangRepository.GetOrderHistory(user.MaNd);

            ViewBag.OrderHistory = lstDonHangSp;

            ViewBag.active = "personal";
            return View();
        }
        public IActionResult UpdateEmail()
        {
            return View();
        }
        public async Task<IActionResult> UpdatePhone()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(string email, string oldPass, string newPass, string comfPass)
        {
            if (newPass != comfPass)
            {
                SetAlert("Mật khẩu xác nhận không khớp", "warning");
            }
            else
            {
                AlertMessage result = await _nguoiDungRepository.ChangePass(email, oldPass, newPass);
                if (result.Type == "success")
                {
                    SetAlert(result.Message, result.Type);

                    return RedirectToAction("Index");
                }
                SetAlert(result.Message, result.Type);
            }
            ViewBag.hisOldPass = oldPass;
            ViewBag.hisNewPass = newPass;
            ViewBag.hisComfPass = comfPass;

            Nguoidung account = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session.GetString("Account"));
            if (account == null)
            {
                SetAlert("Bạn chưa đăng nhập", "warning");
                return RedirectToAction("Index", "Home");
            }    
            Nguoidung currentUser = await _nguoiDungRepository.GetById(account.MaNd);
            ViewBag.User = currentUser;

            ViewBag.active = "password";
            return View("Index");
        }

        public async Task<IActionResult> ChangePersonal(string email, string name, string gioiTinh, string phone, string address, IFormFile avatar)
        {
            try
            {
                Nguoidung user = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session.GetString("Account"));
                Nguoidung oldUser = await _nguoiDungRepository.GetById(user.MaNd);

                if (oldUser.Avatar != null)
                    _cloudinary_Net.Remove(oldUser.Avatar);

                if(avatar!=null)
                {
                    string urlAvatar = _cloudinary_Net.Upload(avatar, "ND");
                    oldUser.Avatar = urlAvatar;
                }    

                oldUser.Email = email;
                oldUser.HoTen = name;
                oldUser.SoDienThoai = phone;
                oldUser.DiaChi = address;
                oldUser.GioiTinh = bool.Parse(gioiTinh);
                

                await _nguoiDungRepository.Update(oldUser);
                user.HoTen = name;
                HttpContext.Session.SetString("Account", JsonSerializer.Serialize(oldUser));
                SetAlert("Cập nhật thông tin cá nhân thành công", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                SetAlert($"Cập nhật thông tin cá nhân thất bại: {ex.Message}", "error");
                return RedirectToAction("Index");
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> SendOTP_Json(string email)
        {

            AlertMessage result = await _sendOTP.SendOTPByEmail(email, email);
            if (result.Type == "success")
            {
                HttpContext.Session.SetString("OTP", JsonSerializer.Serialize(new { email, otpCode = result.Message }));

                return Json(new { success = true, message = "Mã OTP đã được gửi" });
            }

            return Json(new { success = false, message = result.Message });
        }
        [HttpPost]
        public async Task<IActionResult> SendOTPEmail(string email)
        {
            bool isExist = await _nguoiDungRepository.EmailIsExist(email);
            if (isExist)
            {
                return Json(new { success = false, message = "Email đã đăng ký tài khoản" });
            }
            else
            {
                AlertMessage alertMessage = await _sendOTP.SendOTPByEmail(email, email);
                if (alertMessage.Type == "success")
                {
                    OTPCode data = new OTPCode()
                    {
                        email = email,
                        code = alertMessage.Message
                    };

                    HttpContext.Session.SetString("OTP", System.Text.Json.JsonSerializer.Serialize(data));

                    return PartialView("_SendOTPEmail", email);
                }
                else
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi" });
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendOTPPhone(string phone)
        {
                AlertMessage alertMessage = await _sendOTP.SendOTPByPhone(phone);
                if (alertMessage.Type == "success")
                {
                    return PartialView("_SendOTPPhone", phone);
                }
                else
                {
                    return Json(new { success = false, message = "Đã xảy ra lỗi" });
                }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTPByPhone(string phone, string otpCode)
        {
           AlertMessage alertMessage = _sendOTP.CheckOTPByPhone(phone, otpCode);
            
            if (alertMessage.Type == "success")
            {
                Nguoidung user = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session.GetString("Account"));
                Nguoidung oldUser = await _nguoiDungRepository.GetById(user.MaNd);

                await _nguoiDungRepository.UpdatePhone(user.MaNd, phone);
                HttpContext.Session.Remove("OTPCode");
                return Json(new { success = true, message = "Xác thực số điện thoại thành công", redirectUrl = Url.Action("Index", "Account", new { area = "Customer" }) });
            }
            else
                return Json(new { success = false, message = "Mã OTP không đúng", redirectUrl = "" });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otpCode, string email)
        {
            var otpJson = HttpContext.Session.GetString("OTP");
            if (string.IsNullOrEmpty(otpJson)) //check otp từ session
            {
                SetAlert("Mã OTP chưa được gửi", "warning");
                return RedirectToAction("NhapMaOTP", new { email = email });
            }
            //nếu otp từ session tồn tại
            OTPCode OTP = System.Text.Json.JsonSerializer.Deserialize<OTPCode>(otpJson);

            if (email == OTP.email && otpCode == OTP.code)
            {
                Nguoidung user = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session.GetString("Account"));
                Nguoidung oldUser = await _nguoiDungRepository.GetById(user.MaNd);

                await _nguoiDungRepository.UpdateEmail(user.MaNd, email);
                HttpContext.Session.Remove("OTPCode");
                return Json(new { success = true, message = "Xác thực email thành công", redirectUrl = Url.Action("Index", "Account", new { area = "Customer" }) });
            }
            else
                return Json(new { success = false, message = "Mã OTP không đúng", redirectUrl = "" });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            SetAlert("Đăng xuất thành công", "success");
            return RedirectToAction("Index", "Home");
        }
    }
}
