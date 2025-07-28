using Microsoft.AspNetCore.Mvc;
using Twilio.Rest.Trunking.V1;
using WebBanThuocBVTV.Models;

namespace WebBanThuocBVTV.Areas.Shared.Controllers
{
    public class BreadcrumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Back(string text)
        {
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                var breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<Stack<BreadcrumItem>>(breadCrum);
                while(true)
                {
                    if (breadcrumStack.Count > 0)
                    {
                        BreadcrumItem breadcrumItem = breadcrumStack.Pop(); // Xóa phần tử cuối cùng
                        if(breadcrumItem.Text == text)
                        {
                            return Redirect(breadcrumItem.Url); // Chuyển hướng đến URL của phần tử cuối cùng
                        }
                    }
                }    
            }
            return RedirectToAction("Index", "Home"); // Nếu không tìm thấy, chuyển hướng về trang chủ
        }
        public bool Add(BreadcrumItem breadcrumItem)
        {
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                //Tạo session lưu breadcrum
                Stack<BreadcrumItem> breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<Stack<BreadcrumItem>>(breadCrum);
                breadcrumStack.Push(breadcrumItem); // Thêm phần tử mới vào stack
                return true;
            }
            return false;
        }
    }
}
