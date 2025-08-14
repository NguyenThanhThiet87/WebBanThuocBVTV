namespace WebBanThuocBVTV.Helper
{
    public class OTPCode
    {
        public string email { get; set; }
        public string code { get; set; }

    }
    public class OTPCodePhone
    {
        public string phone { get; set; }
        public string code { get; set; }
    }
    public class OTPStatus
    {
        public string email { get; set; }
        public bool status { get; set; }
    }
}
