using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhomSanPhamRepository : IRepository<Nhomsanpham>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public Task<AlertMessage> Add(Nhomsanpham entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateId()
        {
           throw new NotImplementedException();
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Nhomsanpham>> GetAllAsync()
        {
            List<Nhomsanpham> lstNSp = await ContextDB.Nhomsanphams.ToListAsync();
            return lstNSp;
        }

        public Task<AlertMessage> Update(Nhomsanpham entity)
        {
            throw new NotImplementedException();
        }
    }
}
