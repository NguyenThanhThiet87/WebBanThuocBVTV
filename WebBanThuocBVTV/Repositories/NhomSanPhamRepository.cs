using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhomSanPhamRepository : IRepository<Nhomsanpham>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public Task<bool> Add(Nhomsanpham entity)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateId()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Nhomsanpham>> GetAllAsync()
        {
            List<Nhomsanpham> listSp = await ContextDB.Nhomsanphams.ToListAsync();
            return listSp;
        }

        public Task<bool> Update(Nhomsanpham entity)
        {
            throw new NotImplementedException();
        }
    }
}
