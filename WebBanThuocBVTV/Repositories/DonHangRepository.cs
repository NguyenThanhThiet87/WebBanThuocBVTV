using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class DonHangRepository : IRepository<DonhangSanpham>
    {
        private readonly WebBanThuocBvtvContext _contextDB;
        private readonly IdGeneratorHelper _idGeneratorHelper = new IdGeneratorHelper();
        public DonHangRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public async Task<AlertMessage> Add(List<DonhangSanpham> lsDhSp, Donhang donHang)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                await _contextDB.Donhangs.AddAsync(donHang);
                foreach (var dhSp in lsDhSp)
                {
                    await _contextDB.DonhangSanphams.AddAsync(dhSp);
                }
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Đặt hàng thành công";
            } catch(Exception ex)
            {
                alertMessage.Type = "warning";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public Task<AlertMessage> Add(DonhangSanpham entity)
        {
            throw new NotImplementedException();
        }

        public string CreateId()
        {
            string newMaDh = _idGeneratorHelper.GenerateOrderCode();
            return newMaDh;
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<DonhangSanpham>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Donhang Get(string id)
        {
            return _contextDB.Donhangs.Include(dh => dh.MaNdNavigation).Include(dh=>dh.MaTrangThaiNavigation).Include(dh=>dh.DonhangSanphams).ThenInclude(dhsp=>dhsp.MaSanPhamNavigation).ThenInclude(sp=>sp.Hinhanhs).FirstOrDefault(dh => dh.MaDonHang == id);
        }
        public async Task<List<Donhang>> GetOrderHistory(string maNd)
        {
            List<Donhang> lst = await _contextDB.Donhangs.Where(dh => dh.MaNd == maNd).Include(dh=>dh.DonhangSanphams).ThenInclude(dhsp=>dhsp.MaSanPhamNavigation).Include(dh=>dh.MaTrangThaiNavigation).OrderByDescending(dh=>dh.NgayLap).ToListAsync();
            return lst;
        }
        public Task<AlertMessage> Update(DonhangSanpham entity)
        {
            throw new NotImplementedException();
        }
    }
}
