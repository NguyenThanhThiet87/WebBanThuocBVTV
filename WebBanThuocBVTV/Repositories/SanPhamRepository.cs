using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class SanPhamRepository : IRepository<Sanpham>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();

        public async Task<bool> Add(Sanpham entity)
        {
            try
            {
                await ContextDB.Sanphams.AddAsync(entity);
                await ContextDB.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

            }
            return false;
        }

        public async Task<string> CreateId()
        {
            string newMaSp = String.Empty;
            var lastMaSp = await ContextDB.Sanphams.OrderByDescending(sp => sp.MaSanPham).Select(sp => sp.MaSanPham).FirstOrDefaultAsync();
            if (lastMaSp == null)
                newMaSp = "sp0001";
            else
                newMaSp = "sp" + (int.Parse(lastMaSp.ToString().Substring(2)) + 1).ToString("D4");
            return newMaSp;
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Sanpham>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Sanpham entity)
        {
            throw new NotImplementedException();
        }
        public async Task<int> Count()
        {
            int count = await ContextDB.Sanphams.CountAsync();
            return count;
        }
    }
}
