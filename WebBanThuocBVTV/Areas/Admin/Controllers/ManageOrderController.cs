using Microsoft.AspNetCore.Mvc;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    public class ManageOrderController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
