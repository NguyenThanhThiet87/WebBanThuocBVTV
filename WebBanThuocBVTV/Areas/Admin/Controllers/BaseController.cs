using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var json = HttpContext.Session.GetString("SideBar");
            if (!String.IsNullOrEmpty(json))
            {
                SideBar currentSidebar = System.Text.Json.JsonSerializer.Deserialize<SideBar>(json);
                ViewBag.CurrentSideBar = currentSidebar;
            }
            else
            {
                ViewBag.CurrentSideBar = SideBar.TongQuan;
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

        public void SavePointSideBar(SideBar sidebar)
        {
            try
            {
                HttpContext.Session.SetString("SideBar", System.Text.Json.JsonSerializer.Serialize(sidebar));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
