using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //Lấy thông tin breadcrum
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                List<BreadcrumItem> breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<List<BreadcrumItem>>(breadCrum);

                ViewBag.Breadcrum = breadcrumStack;
            }

            // Lấy thông tin user từ session
            var userJson = HttpContext.Session.GetString("Account");
            if (!string.IsNullOrEmpty(userJson))
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<Nguoidung>(userJson);
                ViewBag.Account = user;
            }
            else
            {
                ViewBag.Account = null;
            }

            ViewBag.IndexPage = HttpContext.Session.GetString("IndexPage");//định vị vị trí hiện tại của trang Home hay product

            base.OnActionExecuted(context);
        }

        protected void SetAlert(string message, string type)
        {
            var alert = new AlertMessage { Message = message, Type = type };
            TempData["Alert"] = JsonConvert.SerializeObject(alert);
        }
        public bool AddBreadcrum(BreadcrumItem breadcrumItem)
        {
            var breadCrum = HttpContext.Session?.GetString("Breadcrum") ?? "";
            if (string.IsNullOrEmpty(breadCrum))
            {
                //Tạo session lưu breadcrum
                List<BreadcrumItem> breadcrumStack = new List<BreadcrumItem>();

                if (!breadcrumStack.Contains(breadcrumItem))
                    breadcrumStack.Add(breadcrumItem);

                HttpContext.Session.SetString("Breadcrum", System.Text.Json.JsonSerializer.Serialize(breadcrumStack));

                ViewBag.Breadcrum = breadcrumStack;
                return true;
            }
            else
            {
                //Tạo breadcrum
                List<BreadcrumItem> breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<List<BreadcrumItem>>(breadCrum);
                if (breadcrumStack.Contains(breadcrumItem) == false)
                    breadcrumStack.Add(breadcrumItem); // Thêm phần tử mới vào stack
                HttpContext.Session.SetString("Breadcrum", System.Text.Json.JsonSerializer.Serialize(breadcrumStack));
                return true;
            }
            return false;
        }
        public IActionResult BackBreadcrum(string text)
        {
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                var breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<List<BreadcrumItem>>(breadCrum);

                for (int i = breadcrumStack.Count - 1; i >= 0; i--)
                {
                    if (breadcrumStack[i].Text == text)
                    {
                        HttpContext.Session.SetString("Breadcrum", System.Text.Json.JsonSerializer.Serialize(breadcrumStack));
                        return Redirect(breadcrumStack[i].Url);
                    }
                    breadcrumStack.RemoveAt(i);
                }
            }
            return RedirectToAction("Index", "Home"); // Nếu không tìm thấy, chuyển hướng về trang chủ
        }

        public BreadcrumItem TopBreadcrum()
        {
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                var breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<List<BreadcrumItem>>(breadCrum);

                return breadcrumStack[breadcrumStack.Count - 1];
            }
            return null;
        }
        public bool RemoveBreadcrum(string text)
        {
            var breadCrum = HttpContext.Session.GetString("Breadcrum");
            if (!string.IsNullOrEmpty(breadCrum))
            {
                var breadcrumStack = System.Text.Json.JsonSerializer.Deserialize<List<BreadcrumItem>>(breadCrum);

                for (int i = breadcrumStack.Count - 1; i >= 0; i--)
                {
                    if (breadcrumStack[i].Text == text)
                    {
                        breadcrumStack.RemoveAt(i);
                        HttpContext.Session.SetString("Breadcrum", System.Text.Json.JsonSerializer.Serialize(breadcrumStack));
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
