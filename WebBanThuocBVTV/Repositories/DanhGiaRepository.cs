using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class DanhGiaRepository : IRepository<Danhgia>
    {
        WebBanThuocBvtvContext _contextDB;
        public DanhGiaRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public Task<AlertMessage> Add(Danhgia entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<string> CreateId()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<AlertMessage> Delete(string id)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Danhgia>> GetAllAsync()
        {
            try
            {
                List<Danhgia> lstDanhGia = await _contextDB.Danhgia.ToListAsync();
                return lstDanhGia;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách đánh giá: " + ex.Message);
            }
            return null;
        }

        public Task<AlertMessage> Update(Danhgia entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
