
using static System.Net.Mime.MediaTypeNames;
using System;
using System.Text.Json;

namespace WebBanThuocBVTV.Helper
{
    public class ServerDetection
    {
        private readonly HttpClient _httpClient;
        private readonly string ServerUrl = "http://127.0.0.1:5000/NhanDienBenhCayTrong"; // Địa chỉ URL của server
        public ServerDetection()
        {
            _httpClient = new HttpClient();
        }

        public async Task<(string Class, float conf)> Post(IFormFile img)
        {
            try
            {
                // Kiểm tra file có hợp lệ không
                if (img == null || img.Length == 0)
                {
                    throw new Exception("File ảnh không hợp lệ hoặc rỗng.");
                }

                // Chuẩn bị dữ liệu để gửi
                using var content = new MultipartFormDataContent();
                using var stream = img.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(img.ContentType);
                content.Add(fileContent, "img", img.FileName);

                // Gửi yêu cầu POST đến server
                var response = await _httpClient.PostAsync(ServerUrl, content);

                // Kiểm tra response
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Yêu cầu thất bại với mã trạng thái: {response.StatusCode}");
                }

                // Đọc và parse JSON response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(jsonResponse);
                var root = document.RootElement;

                // Kiểm tra success và lấy Class
                if (root.GetProperty("success").GetBoolean())
                {
                    return (root.GetProperty("Class").GetString(),float.Parse(root.GetProperty("Confidence").ToString()));
                }
                else
                {
                    throw new Exception("Server trả về lỗi: Dự đoán không thành công.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi gửi dữ liệu.", ex);
            }
        }

    }
}
