# Agri T&T | Nền tảng thương mại điện tử thuốc bảo vệ thực vật

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Docker](https://img.shields.io/badge/Container-Docker-2496ED?logo=docker&logoColor=white)](https://www.docker.com/)

Agri T&T là ứng dụng web hỗ trợ kinh doanh thuốc bảo vệ thực vật, được xây dựng cho người mua, nhân viên vận hành và quản trị viên. Hệ thống kết hợp quy trình thương mại điện tử với khả năng nhận diện bệnh cây trồng từ ảnh để hỗ trợ người dùng tìm sản phẩm phù hợp.

> Đây là dự án ASP.NET Core MVC sử dụng SQL Server. Tất cả thông tin nhạy cảm được cấu hình bằng biến môi trường; không lưu secret trong mã nguồn.

**Demo:** [webbanthuocbvtv.onrender.com](https://webbanthuocbvtv.onrender.com)<br>
**Tác giả:** Nguyễn Thanh Thiệt<br>
**Mục đích:** Dự án học tập

## Mục lục

- [Nghiệp vụ và phạm vi](#nghiệp-vụ-và-phạm-vi)
- [Tính năng](#tính-năng)
- [Kiến trúc kỹ thuật](#kiến-trúc-kỹ-thuật)
- [Công nghệ](#công-nghệ)
- [Cấu trúc dự án](#cấu-trúc-dự-án)
- [Yêu cầu môi trường](#yêu-cầu-môi-trường)
- [Cấu hình](#cấu-hình)
- [Chạy local](#chạy-local)
- [Docker và Render](#docker-và-render)
- [Bảo mật](#bảo-mật)
- [Giới hạn triển khai](#giới-hạn-triển-khai)
- [Đóng góp](#đóng-góp)
- [Giấy phép](#giấy-phép)

## Nghiệp vụ và phạm vi

Hệ thống quản lý luồng mua bán từ danh mục sản phẩm đến thanh toán và theo dõi đơn hàng:

```text
Khách hàng → tìm kiếm / nhận diện bệnh → chọn sản phẩm → giỏ hàng
→ tạo đơn hàng → thanh toán VNPay hoặc phương thức hỗ trợ → theo dõi đơn

Quản trị viên → quản lý sản phẩm, danh mục, nhà sản xuất, khách hàng,
nhân viên, đơn hàng, đánh giá và phản hồi
```

Hệ thống không tự động đưa ra khuyến cáo nông nghiệp cuối cùng. Kết quả nhận diện bệnh là dữ liệu hỗ trợ để người dùng tham khảo và chọn sản phẩm.

## Tính năng

### Khách hàng

- Đăng ký, đăng nhập bằng tài khoản nội bộ hoặc Google OAuth.
- Xác thực/khôi phục tài khoản qua OTP email và OTP SMS.
- Duyệt, tìm kiếm, lọc và xem chi tiết sản phẩm.
- Quản lý giỏ hàng, đặt hàng, xem lịch sử và chi tiết đơn hàng.
- Thanh toán trực tuyến qua VNPay.
- Đánh giá, bình luận và phản hồi cho sản phẩm.
- Cập nhật hồ sơ, email, số điện thoại và ảnh đại diện.
- Nhận diện bệnh cây trồng từ ảnh bằng mô hình ONNX và điều hướng đến sản phẩm liên quan.

### Quản trị và vận hành

- Dashboard quản trị.
- Quản lý sản phẩm, nhóm sản phẩm, nhà sản xuất và trạng thái vận hành.
- Quản lý khách hàng, nhân viên và thông tin tài khoản.
- Quản lý đơn hàng, giao dịch, bình luận, đánh giá và phản hồi.
- Lưu trữ hình ảnh qua Cloudinary.

## Kiến trúc kỹ thuật

Ứng dụng sử dụng mô hình ASP.NET Core MVC, phân tách giao diện theo nghiệp vụ và truy cập dữ liệu theo Repository pattern.

```text
Browser
   │
   ▼
ASP.NET Core MVC (.NET 9)
   ├── Areas/Customer  : trải nghiệm khách hàng
   ├── Areas/Admin     : quản trị, vận hành
   ├── Areas/Shared    : đăng nhập và dùng chung
   ├── Controllers     : HTTP request / use-case orchestration
   ├── Repositories    : truy cập và thao tác dữ liệu
   ├── Helpers         : OTP, Cloudinary, VNPay, AI integration
   └── EF Core DbContext
           │
           ▼
       SQL Server

External services: Google OAuth · Gmail/SMTP · Twilio · VNPay · Cloudinary · AI endpoint
```

## Công nghệ

| Nhóm | Công nghệ |
| --- | --- |
| Backend | ASP.NET Core MVC, .NET 9, C# |
| Data | Entity Framework Core 9, Microsoft SQL Server |
| Authentication | Cookie Authentication, Google OAuth |
| UI | Razor Views, Bootstrap, jQuery, AJAX, SweetAlert2 |
| Thanh toán | VNPay |
| Thông báo/Xác thực | MailKit, Twilio, Otp.NET |
| Media | Cloudinary |
| AI | ONNX Runtime, MobileViTv2, ImageSharp, NumSharp |
| Đóng gói | Docker multi-stage build |

## Cấu trúc dự án

```text
.
├── WebBanThuocBVTV/
│   ├── Areas/
│   │   ├── Admin/              # Chức năng quản trị
│   │   ├── Customer/           # Chức năng khách hàng
│   │   └── Shared/             # Xác thực và view dùng chung
│   ├── Helper/                 # VNPay, OTP, Cloudinary, AI integration
│   ├── Models/                 # Entity, DbContext, model thanh toán/AI
│   ├── Repositories/           # Truy cập dữ liệu
│   ├── wwwroot/                # CSS, JavaScript, thư viện frontend
│   ├── Program.cs              # DI, auth, session, routing, startup
│   └── WebBanThuocBVTV.csproj
├── Dockerfile                  # Image production .NET 9
├── .dockerignore
├── .env.example                # Danh sách biến môi trường, không có secret
├── render.yaml                 # Mẫu cấu hình Render Docker
└── DEPLOY_RENDER.md            # Hướng dẫn triển khai Render
```

## Yêu cầu môi trường

- .NET SDK 9.0.
- SQL Server 2019 trở lên hoặc Azure SQL/Cloud SQL for SQL Server tương thích.
- Docker Desktop (khuyến nghị cho kiểm thử image production).
- Tài khoản dịch vụ theo tính năng được sử dụng: Google OAuth, Cloudinary, VNPay, SMTP/Twilio.

## Cấu hình

### Biến môi trường

Sao chép cấu trúc trong `.env.example` và điền giá trị thật vào môi trường chạy. Tên biến dùng `__` để biểu diễn cấp cấu hình của .NET, ví dụ:

```text
ConnectionStrings__DefaultConnection
Authentication__Google__ClientSecret
Cloudinary__api_secret
```

Các nhóm cấu hình chính:

| Nhóm | Mục đích |
| --- | --- |
| `ConnectionStrings__DefaultConnection` | Kết nối SQL Server |
| `Authentication__Google__*` | Google OAuth |
| `Brevo__*` | Gửi OTP email qua Brevo HTTPS API |
| `SmsSettings__*`, `OtpSettings__*` | Xác thực SMS/OTP |
| `Vnpay__*` | Tạo và xác minh giao dịch VNPay |
| `Cloudinary__*` | Upload/xóa ảnh |
| `Server__Url` | Endpoint AI nhận diện bệnh cây |

Không commit `.env`, `appsettings.Development.json`, `bin/` hoặc `obj/`. Các file này đã nằm trong `.gitignore`.

### Local secret storage

Khuyến nghị dùng .NET User Secrets cho máy phát triển:

```powershell
dotnet user-secrets init --project WebBanThuocBVTV
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project WebBanThuocBVTV
```

Lặp lại lệnh `set` cho các khóa cần thiết. Khi chạy trên cloud, đặt cùng các key này trong trang quản lý environment variables của nền tảng.

## Chạy local

```powershell
# Clone repository
git clone https://github.com/NguyenThanhThiet87/WebBanThuocBVTV.git
cd WebBanThuocBVTV

# Khôi phục package, build và chạy ứng dụng
dotnet restore WebBanThuocBVTV/WebBanThuocBVTV.csproj
dotnet build WebBanThuocBVTV/WebBanThuocBVTV.csproj
dotnet run --project WebBanThuocBVTV/WebBanThuocBVTV.csproj
```

Sau khi chạy, truy cập URL được Kestrel in trong terminal.

> SQL Server local có thể dùng Windows Authentication. Khi chạy bằng Docker hoặc cloud, dùng SQL Authentication và một endpoint SQL Server có thể truy cập từ container.

## Docker và Render

Build image:

```powershell
docker build -t web-ban-thuoc-bvtv .
```

Triển khai Render:

1. Push source và `Dockerfile` lên nhánh triển khai (mặc định `master`).
2. Render → **New → Web Service** → chọn repository và nhánh.
3. Chọn runtime **Docker**, Dockerfile Path `./Dockerfile`, instance type phù hợp.
4. Thêm environment variables trực tiếp trong Render; không upload `.env` vào repository.
5. Cập nhật Google OAuth redirect URI và VNPay callback URL theo domain Render.

Tài liệu chi tiết: [`DEPLOY_RENDER.md`](DEPLOY_RENDER.md).

## Bảo mật

- Secret chỉ được đọc từ biến môi trường thông qua `IConfiguration`.
- Application dừng khởi động và chỉ nêu **tên key bị thiếu** khi thiếu cấu hình bắt buộc; không ghi giá trị secret vào log.
- Bật `Encrypt=True` cho chuỗi kết nối SQL Server production và sử dụng chứng chỉ TLS hợp lệ.
- Luôn dùng SQL user có quyền tối thiểu cần thiết; không dùng tài khoản quản trị database cho ứng dụng.
- Thay (rotate) ngay các token đã từng bị lộ trong log, commit, ảnh chụp màn hình hoặc trao đổi công khai.
- Cấu hình callback/redirect URL theo HTTPS domain production trước khi bật Google OAuth và VNPay.

## Giới hạn triển khai

- `Server__Url` không được trỏ đến `127.0.0.1` khi app chạy cloud; AI service cần được deploy độc lập và cấp URL có thể truy cập.
- Render Free có thể sleep khi không có traffic và hạn chế SMTP outbound. Cần chọn dịch vụ phù hợp hoặc email API HTTPS nếu OTP email là yêu cầu production.
- Filesystem container là tạm thời; dùng Cloudinary cho media và SQL Server cho dữ liệu lâu dài.

## Đóng góp

Đây là dự án học tập cá nhân. Nếu muốn đề xuất cải tiến:

1. Fork repository và tạo branch từ `master`.
2. Không đưa secret, file build, dữ liệu production, `bin/` hoặc `obj/` vào commit.
3. Chạy `dotnet build` trước khi tạo pull request.
4. Mô tả rõ thay đổi nghiệp vụ, ảnh hưởng database và biến môi trường mới (nếu có).

## Giấy phép

© 2026 Nguyễn Thanh Thiệt. Dự án được phát triển cho mục đích học tập. **All rights reserved**; không được sử dụng lại, phân phối hoặc triển khai cho mục đích thương mại khi chưa có sự đồng ý của tác giả.

---

Made with ❤️ by **Nguyễn Thanh Thiệt** · [Live demo](https://webbanthuocbvtv.onrender.com)
