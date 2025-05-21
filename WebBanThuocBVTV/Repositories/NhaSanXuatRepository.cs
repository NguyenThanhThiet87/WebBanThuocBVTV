using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhaSanXuatRepository : IRepository<Nhasanxuat>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public Task<bool> Add(Nhasanxuat entity)
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

        public async Task<List<Nhasanxuat>> GetAllAsync()
        {
            List<Nhasanxuat> listNsx = await ContextDB.Nhasanxuats.ToListAsync();
            return listNsx;
        }

        public Task<bool> Update(Nhasanxuat entity)
        {
            throw new NotImplementedException();
        }
    }
}
