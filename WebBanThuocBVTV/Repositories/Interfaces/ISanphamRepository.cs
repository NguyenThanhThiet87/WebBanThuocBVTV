using WebBanThuocBVTV.Models;
namespace WebBanThuocBVTV.Repositories.Interfaces
{
    public interface ISanphamRepository
    {
        string CreateIdSp();
        bool UpdateSp(Sanpham sp);
        bool InsertSp(Sanpham sp);
        bool DeleteSp(string maSo);
        List<Sanpham> GetAllSp();
    }
}
