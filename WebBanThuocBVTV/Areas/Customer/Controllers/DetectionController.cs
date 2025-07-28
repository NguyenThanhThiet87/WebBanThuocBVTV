using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DetectionController : BaseController
    {
        private readonly IConfiguration _config;
        private ServerDetection serverDetection = new ServerDetection();

        public DetectionController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            AddBreadcrum(new BreadcrumItem() { Text = "Nhận Diện", Url = Url.Action("Index","Detection",new {area = "Customer"}) });//thêm vào breadcrum

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Detection(IFormFile img)
        {
            try
            {
                var (result, conf)  = await serverDetection.Post(img);

                string base64Image = "";
                using (var memoryStream = new MemoryStream())
                {
                    await img.CopyToAsync(memoryStream);
                    byte[] imageBytes = memoryStream.ToArray();
                    base64Image = $"data:{img.ContentType};base64,{Convert.ToBase64String(imageBytes)}";
                }

                ViewBag.Img = base64Image;
                ViewBag.Result = result;
                ViewBag.Conf = conf;
            }
            catch(Exception ex)
            {
                SetAlert("Đã xảy ra lỗi khi gửi dữ liệu: " + ex.Message, "error");
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
