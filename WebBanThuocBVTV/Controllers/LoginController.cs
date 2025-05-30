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

namespace WebBanThuocBVTV.Controllers
{
    public class LoginController : BaseController
    {
        NguoiDungRepository nguoiDungRepository = new NguoiDungRepository();
        private readonly IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
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

        private async Task<IActionResult> NhapMaOTP(string email, string flow)
        {
            if (string.IsNullOrEmpty(email))
            {
                SetAlert("Email không hợp lệ", "error");
                return RedirectToAction(flow == "Forget" ? "ForgetPass" : "NhapMaOTP");
            }

            // Kiểm tra email dựa trên luồng
            if (flow == "Register" && await nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Email đã được đăng ký", "error");
                return RedirectToAction("Register");
            }
            else if (flow == "Forget" && !await nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Email chưa được đăng ký", "error");
                return RedirectToAction("ForgetPass");
            }

            // Gửi mã OTP
            AlertMessage result = await SendOTP(email, "Anh/Chị");
            if (result.Type == "error")
            {
                SetAlert(result.Message, result.Type);
                return RedirectToAction(flow == "Forget" ? "ForgetPass" : "Register");
            }

            // Lưu OTP và flow vào session
            HttpContext.Session.SetString("OTPCode", result.Message);
            HttpContext.Session.SetString("Flow", flow);
            ViewBag.Email = email;
            return View("NhapMaOTP");
        }
        public async Task<AlertMessage> SendOTP(string email, string name)
        {
            AlertMessage alerMessage = new AlertMessage();

            try
            {
                // Tạo mã OTP
                // Tạo khóa bí mật (secret key) dạng byte array (base32 decode hoặc generate mới)
                byte[] secretKey = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
                // Khởi tạo đối tượng Totp với khóa bí mật, tùy chọn thuật toán băm, kích thước mã, thời gian bước (step)
                var totp = new Totp(secretKey, step: 30, totpSize: 6, mode: OtpHashMode.Sha1);
                // Tính toán mã OTP dựa trên thời gian hiện tại
                string otpCode = totp.ComputeTotp(); // Mã 6 chữ số

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config["SmtpSettings:Name"], _config["SmtpSettings:Mail"]));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = "Mã xác thực OTP của bạn";

                message.Body = new TextPart("plain")
                {
                    Text = $@"Xin chào {name},

Mã OTP của bạn là: {otpCode}

Vui lòng sử dụng mã này để hoàn tất quá trình xác thực. Mã sẽ hết hạn sau 5 phút.

Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.

Cảm ơn bạn,
Agri T&T"
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_config["SmtpSettings:Mail"], _config["SmtpSettings:Password"]);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                alerMessage.Type = "success";
                alerMessage.Message = $"{otpCode}";
            }
            catch (Exception ex)
            {
                alerMessage.Type = "error";
                alerMessage.Message = "Gửi mã OTP thất bại: " + ex.Message;
            }
            return alerMessage;
        }

        [HttpPost]
        public IActionResult VerifyOTP(string otpCode, string email)
        {
            IActionResult actionReturned;

            string flow = HttpContext.Session.GetString("Flow") ?? "Register";
            switch(flow)
            {
                case "Forget":
                    actionReturned= RedirectToAction("ForgetPass");
                    break;
                case "Register":
                    actionReturned = RedirectToAction("InputInfoAccount");
                    break;
                default:
                    actionReturned = RedirectToAction("Index");
                    break;
            }
            HttpContext.Session.Remove("Flow"); // Xóa flow sau khi xử lý

            string? otpSession = HttpContext.Session.GetString("OTPCode");
            Console.WriteLine(otpSession + ": " + otpCode);
            if (otpSession.IsNullOrEmpty())
            {
                SetAlert("Mã OTP không hợp lệ hoặc đã hết hạn", "error");
                return RedirectToAction("NhapMaOTP");
            }
            if (otpCode == otpSession)
            {
                HttpContext.Session.Remove("OTPCode");
                SetAlert("Xác thực email thành công", "success");
                TempData["Email"] = email;
                return actionReturned;
            }
            else
                SetAlert("Mã OTP không đúng", "error");
            return View("NhapMaOTP");
        }
        public  IActionResult InputInfoAccount()
        {
            string email = TempData["Email"]?.ToString() ?? "Null"; ;
            ViewBag.Email = email;
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> LoginAccount(string email, string password)
        {
            Nguoidung nguoiDung = await nguoiDungRepository.Login(email,password);
            if(nguoiDung == null)
            {
                SetAlert("Tài khoản hoặc mật khẩu không đúng", "error");
                return RedirectToAction("Index");
            }
            SetAlert("Đăng nhập thành công", "success");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount(string name, string email, string password)
        {
            Nguoidung nguoiDung = new Nguoidung();
            nguoiDung.MaNd = await nguoiDungRepository.CreateId();
            nguoiDung.HoTen = name;
            nguoiDung.Email = email;
            
            nguoiDung.PassWord = password;
            nguoiDung.MaVaiTro = "KH";

            AlertMessage result = await nguoiDungRepository.Add(nguoiDung);
            SetAlert(result.Message, result.Type);

            return RedirectToAction("Index");
        }
        //LOGIN WITH GOOGLE
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("SignInGoogle", "Login") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        [HttpGet]
        [Route("/SignInGoogle")]
        public async Task<IActionResult> SignInGoogle()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                SetAlert("Đăng nhập Google thất bại", "error");
                return RedirectToAction("Index");
            }

            var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                SetAlert("Không lấy được email từ Google", "error");
                return RedirectToAction("Index");
            }
            //var nguoiDung = await nguoiDungRepository.GetByEmail(email);
            //if (nguoiDung == null)
            //{
            //    nguoiDung = new Nguoidung
            //    {
            //        MaNd = await nguoiDungRepository.CreateId(),
            //        HoTen = name ?? "Google User",
            //        Email = email.ToLowerInvariant(),
            //        PassWord = "",
            //        MaVaiTro = "KH"
            //    };
            //    var result = await nguoiDungRepository.Add(nguoiDung);
            //    if (result.Type == "error")
            //    {
            //        SetAlert(result.Message, "error");
            //        return RedirectToAction("Index");
            //    }
            //}
            //HttpContext.Session.SetString("UserEmail", nguoiDung.Email);
            //HttpContext.Session.SetString("UserName", nguoiDung.HoTen);
            SetAlert("Đăng nhập Google thành công", "success");
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> NhapMaOTP_Forget(string email)
        {
            HttpContext.Session.SetString("Flow", "Forget"); // Xóa mã OTP cũ nếu có
            if (!await nguoiDungRepository.EmailIsExist(email))
            {
                SetAlert("Email chưa được đăng ký", "error");
                return RedirectToAction("ForgetPass");
            }

            AlertMessage result = await SendOTP(email, "Anh/Chị");
            if (result.Type == "error")
            {
                SetAlert(result.Message, result.Type);
                return RedirectToAction("ForgetPass");
            }
            else
            {
                HttpContext.Session.SetString("OTPCode", result.Message); // Lưu mã OTP vào session
            }
            ViewBag.Email = email;
            return View("NhapMaOTP");
        }

    }
}
