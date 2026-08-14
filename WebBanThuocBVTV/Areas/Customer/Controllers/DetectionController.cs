using Microsoft.AspNetCore.Mvc;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System.Security.Cryptography.Xml;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Advanced;
using WebBanThuocBVTV.Models.Detection;
using NumSharp;

namespace WebBanThuocBVTV.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class DetectionController : BaseController
    {
        static readonly string[] LabelsViSentences = new[]
    {
    "Cây Anh đào khỏe mạnh",
    "Cây Anh đào có thể bị bệnh phấn trắng",
    "Cây Bí có thể bị bệnh phấn trắng",
    "Cây Cà chua có thể bị bệnh Cháy lá muộn",
    "Cây Cà chua có thể bị bệnh Cháy lá sớm",
    "Cây Cà chua có thể bị bệnh Đốm lá Septoria",
    "Cây Cà chua có thể bị bệnh Đốm vi khuẩn",
    "Cây Cà chua có thể bị bệnh Đốm mục tiêu",
    "Cây Cà chua có thể bị bệnh Virus khảm cà chua",
    "Cây Cà chua khỏe mạnh",
    "Cây Cà chua có thể bị bệnh Mốc lá",
    "Cây Cà chua có thể bị bệnh Nhện đỏ hai chấm",
    "Cây Cà chua có thể bị bệnh Virus xoăn vàng lá",
    "Cây Cam có thể bị bệnh Vàng lá gân xanh (Huanglongbing)",
    "Cây Đào có thể bị bệnh Đốm vi khuẩn",
    "Cây Đào khỏe mạnh",
    "Cây Đậu nành khỏe mạnh",
    "Cây Dâu tây có thể bị bệnh Cháy lá",
    "Cây Dâu tây khỏe mạnh",
    "Cây Khoai tây khỏe mạnh",
    "Cây Khoai tây có thể bị bệnh Mốc sớm",
    "Cây Khoai tây có thể bị bệnh Mốc Sương",
    "Cây Lúa có thể bị bệnh Bạc lá",
    "Cây Lúa có thể bị bệnh Đạo ôn",
    "Cây Lúa có thể bị bệnh Đốm nâu",
    "Cây Lúa có thể bị bệnh Bệnh tungro",
    "Cây Mâm xôi khỏe mạnh",
    "Cây Ngô có thể bị bệnh Cháy lá phương Bắc",
    "Cây Ngô có thể bị bệnh Đốm lá xám",
    "Cây Ngô khỏe mạnh",
    "Cây Ngô có thể bị bệnh Rỉ sét thông thường",
    "Cây Nho có thể bị bệnh Cháy lá (Isariopsis)",
    "Cây Nho khỏe mạnh",
    "Cây Nho có thể bị bệnh Esca (Sởi đen)",
    "Cây Nho có thể bị bệnh Thối đen",
    "Cây Ớt chuông có thể bị bệnh Đốm vi khuẩn",
    "Cây Ớt chuông khỏe mạnh",
    "Cây Sắn có thể bị bệnh Bạc lá vi khuẩn",
    "Cây Sắn có thể bị bệnh Khảm sắn",
    "Cây Sắn khỏe mạnh",
    "Cây Sắn có thể bị bệnh Sọc xanh",
    "Cây Sắn có thể bị bệnh Sọc nâu",
    "Cây Táo có thể bị bệnh Đốm đen (Apple scab)",
    "Cây Táo khỏe mạnh",
    "Cây Táo có thể bị bệnh Gỉ sét cedar-táo",
    "Cây Táo có thể bị bệnh Thối đen",
    "Cây Việt quất khỏe mạnh"
    };

        private readonly IConfiguration _config;
        private readonly ServerDetection serverDetection;
        private static readonly string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "Models", "Detection", "MobileViTv2.onnx");
        public DetectionController(IConfiguration config)
        {
            _config = config;
            serverDetection = new ServerDetection(config);
        }

        public IActionResult Index()
        {
            try
            {
                AddBreadcrum(new BreadcrumItem() { Text = "Nhận Diện", Url = Url.Action("Index", "Detection", new { area = "Customer" }) });//thêm vào breadcrum

                return View();
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Detection(IFormFile img)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                ResultDetection result = InferenceModel(img);
                return Json(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                SetAlert("Đã xảy ra lỗi khi gửi dữ liệu: " + ex.Message, "error");
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ResultDetection InferenceModel(IFormFile file)
        {
            try
            {
                using InferenceSession session = new InferenceSession(modelPath);

                // Tiền xử lý
                float[] inputData = PreprocessImage(file).ToArray();

                long[] inputShape = { 1, 3, 256, 256 };

                using var inputOrtValue = OrtValue.CreateTensorValueFromMemory(inputData, inputShape);

                var inputSession = new Dictionary<string, OrtValue>
                {
                  { "pixel_values", inputOrtValue }
                };

                using var runOptions = new RunOptions();

                var outputsession = session.Run(runOptions, inputSession, new[] { "logits" });

                var outputToFeed = outputsession[0].GetTensorDataAsSpan<float>();
                float[] logits = outputToFeed.ToArray();
                ResultDetection result = PostProcessInfer(logits);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DenseTensor<float> PreprocessImage(IFormFile file, int size = 256)
        {
            try
            {
                float[] mean = { 0.485f, 0.456f, 0.406f };
                float[] std = { 0.229f, 0.224f, 0.225f };

                using var stream = file.OpenReadStream();
                using Image<Rgb24> image = Image.Load<Rgb24>(stream);

                image.Mutate(x => x.Resize(size, size));

                var tensor = new DenseTensor<float>(new[] { 1, 3, size, size });

                for (int y = 0; y < size; y++)
                {
                    var row = image.DangerousGetPixelRowMemory(y).Span;

                    for (int x = 0; x < size; x++)
                    {
                        var pixel = row[x];
                        // Chuyển đổi pixel sang giá trị tensor
                        float r = pixel.R / 255f;
                        float g = pixel.G / 255f;
                        float b = pixel.B / 255f;
                        // Chuẩn hóa giá trị pixel
                        tensor[0, 0, y, x] = (pixel.R / 255f - mean[0]) / std[0];
                        tensor[0, 1, y, x] = (pixel.G / 255f - mean[1]) / std[1];
                        tensor[0, 2, y, x] = (pixel.B / 255f - mean[2]) / std[2];
                    }
                }
                return tensor;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ResultDetection PostProcessInfer(float[] logits)
        {
            try
            {
                var y = np.exp(logits - np.max(logits));
                var probs = y / np.sum(y);

                float[] p = probs.ToArray<float>();

                int best = Array.IndexOf(p, p.Max());

                ResultDetection result = new ResultDetection
                {
                    NameInference = LabelsViSentences[best],
                    ConfInference = p[best]
                };
                return result;
            }catch(Exception ex)
            {
                throw ex;
            }
        }
    }

}
