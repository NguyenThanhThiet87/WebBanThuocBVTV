using MailKit.Net.Smtp;
using Microsoft.Identity.Client;
using MimeKit;
using OtpNet;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;


namespace WebBanThuocBVTV.Helper
{
    public class SendOTP
    {
        private readonly IConfiguration _config;
        public SendOTP(IConfiguration config)
        {
            _config = config;
        }
        private string CreateOTP()
        {
            // Tạo khóa bí mật (secret key) dạng byte array (base32 decode hoặc generate mới)
            byte[] secretKey = Base32Encoding.ToBytes("JBSWY3DPEHPK3PXP");
            // Khởi tạo đối tượng Totp với khóa bí mật, tùy chọn thuật toán băm, kích thước mã, thời gian bước (step)
            var totp = new Totp(secretKey, step: 30, totpSize: 6, mode: OtpHashMode.Sha1);
            // Tính toán mã OTP dựa trên thời gian hiện tại
            string otpCode = totp.ComputeTotp(); // Mã 6 chữ số
            return otpCode;
        }
        public async Task<AlertMessage> SendOTPByEmail(string email, string name)
        {
            AlertMessage alerMessage = new AlertMessage();

            try
            {
                // Tạo mã OTP
                string otpCode = CreateOTP();

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_config["SmtpSettings:Name"], _config["SmtpSettings:Mail"]));
                message.To.Add(new MailboxAddress(name, email));
                message.Subject = "Mã xác thực OTP của bạn";

                message.Body = new TextPart("plain")
                {
                    Text = $@"Xin chào {name},

Mã OTP của bạn là: {otpCode}

Vui lòng sử dụng mã này để hoàn tất quá trình xác thực. Mã sẽ hết hạn sau 5 phút.

Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.

Cảm ơn bạn,
Agri T&T"
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    await client.AuthenticateAsync(_config["SmtpSettings:Mail"], _config["SmtpSettings:Password"]);

                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                alerMessage.Type = "success";
                alerMessage.Message = $"{otpCode}";
            }
            catch (Exception ex)
            {
                alerMessage.Type = "error";
                alerMessage.Message = "Gửi mã OTP thất bại: " + ex.Message;
            }
            return alerMessage;
        }
        public async Task<AlertMessage> SendOTPByPhone(string phoneNumber)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                string otpCode = CreateOTP();

                                                                          
                alertMessage.Type = "success";
                alertMessage.Message = "Gửi mã OTP thành công";
            }catch(Exception ex)
            {
                    alertMessage.Type = "error";
                    alertMessage.Message = "Gửi mã OTP thất bại: " + ex.Message;
            }
            return alertMessage;
        }



    }

}
