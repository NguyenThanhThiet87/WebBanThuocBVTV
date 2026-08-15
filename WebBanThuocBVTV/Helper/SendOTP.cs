using MailKit.Net.Smtp;
using Microsoft.Identity.Client;
using MimeKit;
using OtpNet;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Verify.V2.Service;

using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;


namespace WebBanThuocBVTV.Helper
{
    public class SendOTP
    {
        private readonly IConfiguration _config;
        public SendOTP(IConfiguration config)
        {
            _config = config;
            TwilioClient.Init(_config["SmsSettings:AccountSID"], _config["SmsSettings:AuthToken"]);
        }
        private string CreateOTP()
        {
            // Tạo khóa bí mật (secret key) dạng byte array (base32 decode hoặc generate mới)
            byte[] secretKey = Base32Encoding.ToBytes(_config["OtpSettings:SecretKey"]!);
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
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
                    client.Timeout = 15000;

                    await client.ConnectAsync(
                        _config["SmtpSettings:Host"],
                        int.Parse(_config["SmtpSettings:Port"]!),
                        MailKit.Security.SecureSocketOptions.StartTls,
                        timeout.Token);

                    await client.AuthenticateAsync(
                        _config["SmtpSettings:Mail"],
                        _config["SmtpSettings:Password"],
                        timeout.Token);

                    await client.SendAsync(message, timeout.Token);
                    await client.DisconnectAsync(true, timeout.Token);
                }
                alerMessage.Type = "success";
                alerMessage.Message = $"{otpCode}";
            }
            catch (OperationCanceledException)
            {
                alerMessage.Type = "error";
                alerMessage.Message = "Không thể kết nối máy chủ gửi email. Vui lòng thử lại sau.";
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
                var verification = VerificationResource.Create(
                                   to: phoneNumber,
                                   channel: "sms",
                                   pathServiceSid: _config["SmsSettings:VerifyServiceSID"]
                                   );

                alertMessage.Type = "success";
                alertMessage.Message = "Mã OTP đã được gửi đến số điện thoại của bạn. Vui lòng kiểm tra tin nhắn SMS.";
                return alertMessage;
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Gửi mã OTP thất bại: " + ex.Message;
                return alertMessage;
            }
        }
        public AlertMessage CheckOTPByPhone(string phoneNumber, string code)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                var serviceSid = _config["SmsSettings:VerifyServiceSID"];
                var check = VerificationCheckResource.Create(
                    to: phoneNumber,
                    code: code,
                    pathServiceSid: serviceSid
                );
                if(check.Status == "approved")
                {
                    alertMessage.Type = "success";
                    alertMessage.Message = "Xác thực thành công";
                }else
                {
                    alertMessage.Type = "error";
                    alertMessage.Message = "Mã OTP không hợp lệ";
                }    

                return alertMessage;
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Gửi mã OTP thất bại: " + ex.Message;
                return alertMessage;
            }
        }

    }

}
