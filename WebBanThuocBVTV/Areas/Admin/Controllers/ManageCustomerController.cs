using Google.Rpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using WebBanThuocBVTV.Repositories.Interfaces;
using X.PagedList.Extensions;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageCustomerController : BaseController
    {
        Cloudinary_Net _cloudinary_Net;
        NguoiDungRepository _nguoiDungRepository;
        SendOTP _sendOTP;
        public ManageCustomerController(NguoiDungRepository nguoiDungRepository, SendOTP sendOTP, IConfiguration _config)
        {
            _nguoiDungRepository = nguoiDungRepository;
            _sendOTP = sendOTP;
            _cloudinary_Net = new Cloudinary_Net(_config);
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                SavePointSideBar(SideBar.KhachHang);

                List<Nguoidung> lstUser = await _nguoiDungRepository.GetAllCustomer();

                ViewBag.TongKh = lstUser.Count;
                ViewBag.KhMoi = lstUser.Where(user => user.NgayTao.Value.Month == DateTime.Now.Month && user.NgayTao.Value.Year == DateTime.Now.Year).Count();
                return View(lstUser);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> DetailCustomer(string maNd)
        {
            try
            {
                Nguoidung nd = await _nguoiDungRepository.GetById(maNd);
                return PartialView("_DetailCustomer", nd);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateCustomer(Nguoidung nd)
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
                    return PartialView("_DetailCustomer", user);
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
        public  IActionResult AddCustomer()
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
            }catch(Exception ex)
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
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendOTPPhone(string phone)
        {
            try
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
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTPPhone(string otp, string phone)
        {
            try
            {
                AlertMessage alertMessage = _sendOTP.CheckOTPByPhone(phone, otp);
                if(alertMessage.Type=="success")
                {
                    OTPStatusPhone data = new OTPStatusPhone { phone = phone, status = true };
                    HttpContext.Session.SetString("verifyPhone", System.Text.Json.JsonSerializer.Serialize(data));
                    return Json(new { success = true, message = "Xác thực thành công" });
                }else
                 {
                    return Json(new { success = false, message = "Mã OTP không hợp lệ" });
                 }
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> InsertCustomer(Nguoidung nd, IFormFile avatar)
        {
            try
            {
                var json = HttpContext.Session.GetString("verifyEmail");
                if (!string.IsNullOrEmpty(json))
                {
                    OTPStatus verifyEmailObject = System.Text.Json.JsonSerializer.Deserialize<OTPStatus>(json);

                    if (verifyEmailObject.email == nd.Email && verifyEmailObject.status)
                    {
                       if(nd.SoDienThoai!=null)
                        {
                            var jsonPhone = HttpContext.Session.GetString("verifyPhone");
                            if (!string.IsNullOrEmpty(jsonPhone))
                            {
                                OTPStatusPhone verifyPhoneObject = System.Text.Json.JsonSerializer.Deserialize<OTPStatusPhone>(jsonPhone);
                                if (verifyPhoneObject.phone == nd.SoDienThoai && verifyPhoneObject.status)
                                {
                                    if (avatar != null)
                                    {
                                        string urlAvatar = _cloudinary_Net.Upload(avatar, "ND");
                                        nd.Avatar = urlAvatar;
                                    }
                                    nd.MaNd = await _nguoiDungRepository.CreateId();
                                    nd.NgayTao = DateTime.Now;
                                    nd.MaVaiTro = "KH";

                                    AlertMessage alertMessage = await _nguoiDungRepository.Add(nd);
                                    SetAlert(alertMessage.Message, alertMessage.Type);
                                    return RedirectToAction("AddCustomer");
                                }
                                else
                                {
                                    SetAlert("Số điện thoại chưa được xác thực", "warning");
                                    ViewBag.avatar = avatar;
                                    return View("AddCustomer", nd);
                                }
                            }
                            else
                            {
                                SetAlert("Số điện thoại chưa được xác thực", "warning");
                                ViewBag.avatar = avatar;
                                return View("AddCustomer", nd);
                            }
                        } else
                        {
                            if (avatar != null)
                            {
                                string urlAvatar = _cloudinary_Net.Upload(avatar, "ND");
                                nd.Avatar = urlAvatar;
                            }
                            nd.MaNd = await _nguoiDungRepository.CreateId();
                            nd.NgayTao = DateTime.Now;
                            nd.MaVaiTro = "KH";

                            AlertMessage alertMessage = await _nguoiDungRepository.Add(nd);
                            SetAlert(alertMessage.Message, alertMessage.Type);
                            return RedirectToAction("AddCustomer");
                        }    
                    }
                    else
                    {
                        SetAlert("Email chưa được xác thực", "warning");
                        ViewBag.avatar = avatar;
                        return View("AddCustomer", nd);
                    }
                }
                SetAlert("Email chưa được xác thực", "warning");
                ViewBag.avatar = avatar;
                return View("AddCustomer", nd);
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SearchCustomer(string keyword, int? page)
        {
            try
            {
                if (page == null)
                    page = 1;

                List<Nguoidung> lstUsers = null;
                if (String.IsNullOrEmpty(keyword))
                {
                    lstUsers = await _nguoiDungRepository.GetAllCustomer();
                }
                else
                    lstUsers = await _nguoiDungRepository.SearchNguoiDung(keyword);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstUsers.Count / pageSize;

                return PartialView("_ListCustomer", lstUsers.ToPagedList(pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> FilterCustomer(string keyword, CategoryCustomer loaiKh, GenderOptions gioiTinh, CreateAtOptions ngayTao, SortOptionsCustomer SortOption, int? page)
        {
            try
            {
                keyword = keyword ?? "";

                if (page == null)
                    page = 1;

                List<Nguoidung> lstCustomers = await _nguoiDungRepository.FilterCustomer(keyword, loaiKh, gioiTinh, ngayTao, SortOption);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstCustomers.Count / pageSize;

                return PartialView("_ListCustomer", lstCustomers.ToPagedList(pageNumber, pageSize));
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<AlertMessage> DeleteCustomer(string maNd)
        {
            try
            {
                AlertMessage alertMessage = await _nguoiDungRepository.Delete(maNd);
                return alertMessage;
            }
            catch (Exception ex)
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
