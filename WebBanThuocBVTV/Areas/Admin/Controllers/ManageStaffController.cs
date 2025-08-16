using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using X.PagedList.Extensions;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageStaffController : BaseController
    {
        Cloudinary_Net _cloudinary_Net;
        NguoiDungRepository _nguoiDungRepository;
        SendOTP _sendOTP;
        public ManageStaffController(NguoiDungRepository nguoiDungRepository, SendOTP sendOTP, IConfiguration _config)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _sendOTP = sendOTP;
            _cloudinary_Net = new Cloudinary_Net(_config);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                SavePointSideBar(SideBar.NhanVien);

                List<Nguoidung> lstUser = await _nguoiDungRepository.GetAllStaff();
                return View(lstUser);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DetailStaff(string maNd)
        {
            try
            {
                Nguoidung nd = await _nguoiDungRepository.GetById(maNd);
                return PartialView("_DetailStaff", nd);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateStaff(Nguoidung nd)
        {
            try
            {
                Nguoidung user = await _nguoiDungRepository.GetById(nd.MaNd);

                user.HoTen = nd.HoTen;
                user.NgaySinh = nd.NgaySinh;
                user.Email = nd.Email;
                user.SoDienThoai = nd.SoDienThoai;
                user.DiaChi = nd.DiaChi;
                user.GioiTinh = nd.GioiTinh;

                AlertMessage alertMessage = await _nguoiDungRepository.Update(user);
                if (alertMessage.Type == "success")
                {
                    return PartialView("_DetailStaff", user);
                }
                else
                {
                    return Json(new { success = false, message = alertMessage.Message });
                }
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult AddStaff()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendOTPEmail(string email)
        {
            try
            {
                bool isExist = await _nguoiDungRepository.EmailIsExist(email);
                if (isExist)
                {
                    return Json(new { success = false, message = "Email đã tồn tại" });
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
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> VerifyOTPEmail(string otp, string email)
        {
            try
            {
                string otpJson = HttpContext.Session.GetString("OTP");
                if (String.IsNullOrEmpty(otpJson))
                {
                    return Json(new { success = false, message = "Mã OTP không hợp lệ" });
                }
                else
                {
                    OTPCode otpSended = System.Text.Json.JsonSerializer.Deserialize<OTPCode>(otpJson);
                    if (email == otpSended.email && otp == otpSended.code)
                    {
                        OTPStatus data = new OTPStatus { email = email, status = true };
                        HttpContext.Session.SetString("verifyEmail", System.Text.Json.JsonSerializer.Serialize(data));
                        return Json(new { success = true, message = "Xác thực thành công" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Mã OTP không hợp lệ" });
                    }
                }
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertStaff(Nguoidung nd, IFormFile avatar)
        {
            try
            {
                var json = HttpContext.Session.GetString("verifyEmail");
                if (!string.IsNullOrEmpty(json))
                {
                    OTPStatus verifyEmailObject = System.Text.Json.JsonSerializer.Deserialize<OTPStatus>(json);

                    if (verifyEmailObject.email == nd.Email && verifyEmailObject.status)
                    {
                        if (avatar != null)
                        {
                            string urlAvatar = _cloudinary_Net.Upload(avatar, "STAF");
                            nd.Avatar = urlAvatar;
                        }

                        nd.MaNd = await _nguoiDungRepository.CreateId();
                        nd.NgayTao = DateTime.Now;
                        nd.MaVaiTro = "NV";

                        AlertMessage alertMessage = await _nguoiDungRepository.AddStaff(nd);
                        SetAlert(alertMessage.Message, alertMessage.Type);
                        return RedirectToAction("AddStaff");
                    }
                    else
                    {
                        SetAlert("Email chưa được xác thực", "warning");
                        ViewBag.avatar = avatar;
                        return View("AddStaff", nd);
                    }
                }
                SetAlert("Email chưa được xác thực", "warning");
                ViewBag.avatar = avatar;
                return View("AddStaff", nd);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchStaff(string keyword, int? page)
        {
            try
            {
                if (page == null)
                    page = 1;

                List<Nguoidung> lstUsers = null;
                if (String.IsNullOrEmpty(keyword))
                {
                    lstUsers = await _nguoiDungRepository.GetAllStaff();
                }
                else
                    lstUsers = await _nguoiDungRepository.SearchNguoiDung(keyword);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstUsers.Count / pageSize;

                return PartialView("_ListStaff", lstUsers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterStaff(string keyword, GenderOptions gioiTinh, CreateAtOptions ngayTao, SortOptionsCustomer SortOption, int? page)
        {
            try
            {
                keyword = keyword ?? "";

                if (page == null)
                    page = 1;

                List<Nguoidung> lstStaffs = await _nguoiDungRepository.FilterStaff(keyword, gioiTinh, ngayTao, SortOption);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstStaffs.Count / pageSize;

                return PartialView("_ListStaff", lstStaffs.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> DeleteStaff(string maNd)
        {
            try
            {
                AlertMessage alertMessage = await _nguoiDungRepository.Delete(maNd);
                return alertMessage;
            }
            catch(Exception ex)
            {
                return new AlertMessage()
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }
    }
}
