using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class TrangThaiRepository : IRepository<Trangthai>
    {
        WebBanThuocBvtvContext _contextDB;

        public TrangThaiRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }

        public Task<AlertMessage> Add(Trangthai entity)
        {
            throw new NotImplementedException();
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Trangthai>> GetAllAsync()
        {
            try
            {
                return _contextDB.Trangthais.ToListAsync();
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public Task<AlertMessage> Update(Trangthai entity)
        {
            throw new NotImplementedException();
        }
    }
}
