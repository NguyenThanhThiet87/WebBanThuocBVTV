using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebBanThuocBVTV.Helper;

namespace WebBanThuocBVTV.Controllers
{
    public class BaseController : Controller
    {
        protected void SetAlert(string message, string type)
        {
            var alert = new AlertMessage { Message = message, Type = type };
            TempData["Alert"] = JsonConvert.SerializeObject(alert);
        }
    }
}
