using WebBanThuocBVTV.Models.VnPay;

namespace WebBanThuocBVTV.Helper.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
