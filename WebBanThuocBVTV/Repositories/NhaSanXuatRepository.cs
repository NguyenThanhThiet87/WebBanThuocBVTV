using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhaSanXuatRepository : IRepository<Nhasanxuat>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public Task<AlertMessage> Add(Nhasanxuat entity)
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

        public async Task<List<Nhasanxuat>> GetAllAsync()
        {
            List<Nhasanxuat> listNsx = await ContextDB.Nhasanxuats.ToListAsync();
            return listNsx;
        }

        public Task<AlertMessage> Update(Nhasanxuat entity)
        {
            throw new NotImplementedException();
        }
    }
}
