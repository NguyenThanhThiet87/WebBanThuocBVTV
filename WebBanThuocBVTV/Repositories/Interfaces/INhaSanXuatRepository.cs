using WebBanThuocBVTV.Models;

namespace WebBanThuocBVTV.Repositories.Interfaces
{
    public interface INhaSanXuatRepository
    {
        string CreateIdSp();
        bool UpdateSp(Sanpham sp);
        bool InsertSp(Sanpham sp);
        bool DeleteSp(string maSo);
        List<Sanpham> GetAllNsp();
    }
}
