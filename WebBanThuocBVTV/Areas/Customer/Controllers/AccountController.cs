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

        public AccountController(SendOTP sendOTP, NguoiDungRepository nguoiDungRepository, DonHangRepository donHangRepository)
        {
            _sendOTP = sendOTP;
            _nguoiDungRepository = nguoiDungRepository;
            _donHangRepository = donHangRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            AddBreadcrum(new BreadcrumItem() { Text = "Tài Khoản", Url = Url.Action("Index", "Account", new {area = "Customer"}) });//thêm vào breadcrum

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
            AlertMessage result= await _sendOTP.SendOTPByPhone("0868642533");
            SetAlert(result.Message, result.Type);
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

        public async Task<IActionResult> ChangePersonal(string email, string name, string gioiTinh, string phone, string address)
        {
            Nguoidung user = JsonSerializer.Deserialize<Nguoidung>(HttpContext.Session.GetString("Account"));
            Nguoidung oldUser = await _nguoiDungRepository.GetById(user.MaNd);


            oldUser.Email = email;
            oldUser.HoTen = name;
            oldUser.SoDienThoai = phone;
            oldUser.DiaChi = address;
            oldUser.GioiTinh = bool.Parse(gioiTinh);

            try
            {
                await _nguoiDungRepository.Update(oldUser);
                user.HoTen = name;
                HttpContext.Session.SetString("Account", JsonSerializer.Serialize(user));
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
        public async Task<IActionResult> VerifyOTP(string email, string otpCode)
        {
            ViewBag.Email = email;
            ViewBag.OtpCode = otpCode;

            //check email tồn tại chưa
            if (await _nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Tài khoản Email đã tồn tại", "warning");
                return View("UpdateEmail");
            }

            var otpJson = HttpContext.Session.GetString("OTP");
            
            if (string.IsNullOrEmpty(otpJson)) //check otp từ session
            {
                SetAlert("Mã OTP chưa được gửi", "warning");
                return View("UpdateEmail");
            }
            //nếu otp từ session tồn tại
            var OTP = JsonSerializer.Deserialize<Dictionary<string, string>>(otpJson);
            if (email == OTP["email"] && otpCode == OTP["otpCode"])
            {
                var account = HttpContext.Session.GetString("Account");
                Nguoidung user = JsonSerializer.Deserialize<Nguoidung>(account);

                try
                {
                    await _nguoiDungRepository.UpdateEmail(user.MaNd, email);
                    SetAlert("Cập nhật Email thành công", "success");
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    SetAlert($"Cập nhật Email thất bại: {ex.Message}", "error");
                    return RedirectToAction("UpdateEmail");
                }
            }
            else
            {
                SetAlert("Mã OTP chưa đúng", "warning");
                return View("UpdateEmail");
            }
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
