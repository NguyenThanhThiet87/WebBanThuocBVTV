# Deploy lên Render

Các khóa bảo mật không còn được lưu trong mã nguồn. Danh sách tên biến nằm ở `.env.example` và `render.yaml`; chỉ điền **giá trị thật** trong Render Dashboard, không commit chúng.

Trước khi commit thay đổi này, bỏ các file vốn đã được Git theo dõi khỏi index (chúng vẫn còn trên máy):

```powershell
git rm -r --cached -- WebBanThuocBVTV/bin WebBanThuocBVTV/obj WebBanThuocBVTV/appsettings.Development.json
```

## Local

Khởi tạo một lần và đưa các giá trị từ `.env.example` vào User Secrets:

```powershell
dotnet user-secrets init --project WebBanThuocBVTV
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=tcp:sql.example.com,1433;Database=WebBanThuoc;User Id=app_user;Password=...;Encrypt=True;TrustServerCertificate=False;" --project WebBanThuocBVTV
dotnet user-secrets set "Authentication:Google:ClientSecret" "..." --project WebBanThuocBVTV
```

Lặp lại `dotnet user-secrets set` cho các biến còn lại, thay `__` trong `.env.example` bằng `:`. User Secrets chỉ dùng cho máy local, không được publish.

## Render

1. Push các thay đổi này và tạo **New > Blueprint** từ `render.yaml`. Nếu tạo bằng Dashboard, chọn **Web Service** rồi chọn runtime **Docker** và Dockerfile Path là `./Dockerfile`.
2. Khi Render hỏi các biến `sync: false`, nhập giá trị thật. Với service đã có sẵn, vào **Environment** và thêm từng biến; Render không hỏi lại các biến mới trong những lần Blueprint sync sau.
3. Giá trị SQL Server đặt tại `ConnectionStrings__DefaultConnection`. Dùng connection string TCP công khai/có private network phù hợp, ví dụ:

   ```text
   Server=tcp:sql.example.com,1433;Database=WebBanThuoc;User Id=app_user;Password=...;Encrypt=True;TrustServerCertificate=False;
   ```

4. Mở firewall SQL Server cho outbound traffic của Render hoặc dùng SQL Server có endpoint công khai an toàn. Không thể dùng `localhost`, `127.0.0.1`, tên SQL Server trong LAN, hoặc `Trusted_Connection=True` từ Render. Dùng SQL authentication, ví dụ `Server=tcp:sql.example.com,1433;Database=WebBanThuoc;User Id=app_user;Password=...;Encrypt=True;TrustServerCertificate=False;`.
5. `Server__Url` không thể là `127.0.0.1` khi deploy. Deploy API nhận diện cây trồng thành một service riêng (ví dụ Docker/Python) và đặt URL HTTPS công khai của service đó vào biến này.
6. Cập nhật Google OAuth authorized redirect URI thành `https://<ten-service>.onrender.com/signin-google`, và VNPay return URL thành `https://<ten-service>.onrender.com/Customer/VnPay/PaymentCallbackVnpay`.

`Program.cs` giờ sẽ dừng ngay với thông báo chỉ nêu tên biến nếu thiếu cấu hình. Nó không in ra giá trị secret.

## Việc cần làm ngay

Các khóa cũ từng được commit cần được thu hồi/tạo lại: Gmail App Password, Google OAuth client secret, Twilio Auth Token, VNPay Hash Secret, Cloudinary API secret và mật khẩu SQL Server. Chỉ xóa khỏi file hiện tại không xóa được chúng khỏi lịch sử Git.
