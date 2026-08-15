using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using OtpNet;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text.Json;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using Twilio.Jwt.AccessToken;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using WebBanThuocBVTV.Areas.Customer.Controllers;

namespace WebBanThuocBVTV.Areas.Shared.Controllers
{
    [Area("Shared")]
    public class LoginController : BaseController
    {
        NguoiDungRepository _nguoiDungRepository;
        GioHangRepository _gioHangRepository;
        private readonly IConfiguration _config;
        private SendOTP _sendOTP;
        public LoginController(IConfiguration config, NguoiDungRepository nguoiDungRepository, GioHangRepository gioHangRepository, SendOTP sendOTP)
        {
            _config = config;
            _nguoiDungRepository = nguoiDungRepository;
            _gioHangRepository = gioHangRepository;
            _sendOTP = sendOTP;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ForgetPass()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NhapMaOTP(string email, string flow)
        {
            if (string.IsNullOrEmpty(email))
            {
                SetAlert("Email không hợp lệ", "error");
                return RedirectToAction(flow == "Forget" ? "ForgetPass" : "NhapMaOTP");
            }

            // Kiểm tra email dựa trên luồng
            if (flow == "Register" && await _nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Email đã được đăng ký", "error");
                return RedirectToAction("Register");
            }
            else if (flow == "Forget" && !await _nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Email chưa được đăng ký", "error");
                return RedirectToAction("ForgetPass");
            }

            // Gửi mã OTP
            AlertMessage result = await _sendOTP.SendOTPByEmail(email, "Anh/Chị");
            if (result.Type == "error")
            {
                SetAlert(result.Message, result.Type);
                return RedirectToAction(flow == "Forget" ? "ForgetPass" : "Register");
            }

            // Lưu OTP và flow vào session
            HttpContext.Session.SetString("OTP", System.Text.Json.JsonSerializer.Serialize(new { email, otpCode = result.Message }));
            HttpContext.Session.SetString("Flow", flow);
            ViewBag.Email = email;
            return View("NhapMaOTP");
        }

        public IActionResult SetNewPass(string email)
        {
            ViewBag.Email = email;  
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string otpCode, string email)
        {
            string actionReturned;

            string flow = HttpContext.Session.GetString("Flow") ?? "Register";
            switch (flow)
            {
                case "Forget":
                    actionReturned = Url.Action("SetNewPass", "Login", new { Email = email });
                    break;
                case "Register":
                    actionReturned = Url.Action("InputInfoAccount", "Login", new { Email = email });
                    break;
                default:
                    actionReturned = Url.Action("Index", "Login", new { Email = email });
                    break;
            }
            HttpContext.Session.Remove("Flow"); // Xóa flow sau khi xử lý

            TempData["Email"] = email;
            var otpJson = HttpContext.Session.GetString("OTP");
            if (string.IsNullOrEmpty(otpJson)) //check otp từ session
            {
                SetAlert("Mã OTP chưa được gửi", "warning");
                return RedirectToAction("NhapMaOTP_Forget", new {email=email});
            }
            //nếu otp từ session tồn tại
            OTPCode OTP = System.Text.Json.JsonSerializer.Deserialize<OTPCode>(otpJson);

            if (email == OTP.email && otpCode == OTP.code)
            {
                HttpContext.Session.Remove("OTPCode");
                return Json(new { success = true, message = "Xác thực email thành công", redirectUrl =  actionReturned });
            }
            else
                return Json(new { success = false, message = "Mã OTP không đúng", redirectUrl = "" });
        }
        public IActionResult InputInfoAccount()
        {
            string email = TempData["Email"]?.ToString() ?? "Null";
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAccount(string email, string password)
        {
            Nguoidung nguoiDung = await _nguoiDungRepository.Login(email, password);
            if (nguoiDung == null)
            {
                SetAlert("Tài khoản hoặc mật khẩu không đúng", "error");
                return RedirectToAction("Index");
            }
            Nguoidung Account = new Nguoidung();
            Account.MaNd = nguoiDung.MaNd;
            Account.HoTen = nguoiDung.HoTen;
            Account.Avatar = nguoiDung.Avatar;
            Account.MaVaiTro = nguoiDung.MaVaiTro;

            HttpContext.Session.SetString("Account", System.Text.Json.JsonSerializer.Serialize(Account));
            switch (nguoiDung.MaVaiTro)
            {
                case "KH":
                    ViewBag.Layout = "~/Views/Shared/WebBanThuocBVTV.cshtml"; // Layout cho khách hàng
                    SetAlert("Đăng nhập thành công - Chào mừng Khách Hàng", "success");
                    return RedirectToAction("Index", "Home", new {area = "Customer"});
                case "NV":
                case "QL":
                    ViewBag.Layout = "~/Views/Shared/WebBanThuocBVTV.cshtml"; // Layout cho khách hàng
                    SetAlert("Đăng nhập thành công - Chào mừng Quản Lý", "success");
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                default:
                    break;
            }
            SetAlert("Đăng nhập thành công", "success");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount(string name, string email, string password)
        {
            Nguoidung nguoiDung = new Nguoidung();
            nguoiDung.MaNd = await _nguoiDungRepository.CreateId();
            nguoiDung.HoTen = name;
            nguoiDung.Email = email;
            nguoiDung.NgayTao = DateTime.Now;
            nguoiDung.PassWord = password;
            nguoiDung.MaVaiTro = "KH";

            AlertMessage result = await _nguoiDungRepository.Add(nguoiDung);
            SetAlert(result.Message, result.Type);

            return RedirectToAction("Index");
        }
        //LOGIN WITH GOOGLE
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action(nameof(SignInGoogle), "Login", new {area="Shared"}) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        public async Task<IActionResult> SignInGoogle()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                SetAlert("Đăng nhập Google thất bại", "error");
                return RedirectToAction("Index");
            }

            //lấy thông tin từ tài khoản google
            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            // Google handler đã ánh xạ subject (sub) vào NameIdentifier. Dùng claim này
            // tránh phụ thuộc vào một request bổ sung đến UserInfo API sau callback.
            var googleId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
            {
                SetAlert("Không lấy được thông tin định danh từ Google. Vui lòng thử lại.", "error");
                return RedirectToAction("Index");
            }


            Nguoidung user = new Nguoidung();
            user.HoTen = email.Split("@")[0].ToString() ?? "Google User";
            user.Email = email.ToLower();
            user.PassWord = "";
            user.GoogleId = googleId;
            user.MaVaiTro = "KH";

            AlertMessage result = await _nguoiDungRepository.LoginWithGoogle(user);


            if (result.Type == "success")
            {
                Nguoidung userUpdated = await _nguoiDungRepository.GetByEmail(email);
                Nguoidung Account = new Nguoidung();
                Account.MaNd = userUpdated.MaNd;
                Account.HoTen = userUpdated.HoTen;
                Account.Avatar = userUpdated.Avatar;
                Account.MaVaiTro = userUpdated.MaVaiTro;

                SetAlert("Đăng nhập thành công - Chào mừng Khách Hàng", "success");
                HttpContext.Session.SetString("Account", System.Text.Json.JsonSerializer.Serialize(Account));
                return RedirectToAction("Index", "Home", new {area="Customer"});
            }
            SetAlert(result.Message, result.Type);
            return RedirectToAction("Index");
        }
        private async Task<string> GetGoogleUserIdFromUserInfo(string accessToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await client.GetAsync("https://www.googleapis.com/userinfo/v2/me");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var data = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                        if (data.ContainsKey("id"))
                        {
                            return data["id"].ToString(); // 'id' là Google User ID (sub)
                        }
                    }
                    else
                    {
                        Console.WriteLine($"UserInfo API lỗi: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gọi UserInfo Endpoint: {ex.Message}");
            }
            return null;
        }
        

        [HttpPost]
        public async Task<IActionResult> SendOTPEmailForForget(string email)
        {
            HttpContext.Session.SetString("Flow", "Forget"); // Xóa mã OTP cũ nếu có
            bool isExist = await _nguoiDungRepository.EmailIsExist(email);
            if (!isExist)
            {
                return Json(new { success = false, message = "Email chưa đăng ký tài khoản" });
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
                    return Json(new { success = false, message = alertMessage.Message });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendOTPEmailForRegister(string email)
        {
            HttpContext.Session.SetString("Flow", "Register"); // Xóa mã OTP cũ nếu có
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
                    return Json(new { success = false, message = alertMessage.Message });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(string email, string newPass, string comfPass)
        {
            if (newPass != comfPass)
            {
                SetAlert("Mật khẩu xác nhận không khớp", "warning");
            }
            else
            {
                AlertMessage result = await _nguoiDungRepository.ChangePassVerified(email, newPass);

                if (result.Type == "success")
                {
                    SetAlert(result.Message, result.Type);

                    return RedirectToAction("Index");
                }
                SetAlert(result.Message, result.Type);
            }

            ViewBag.email = email;
            ViewBag.newPass = newPass;
            ViewBag.comfPass = comfPass;

            return View("SetNewPass");
        }

    }
}
