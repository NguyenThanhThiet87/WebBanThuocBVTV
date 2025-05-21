using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class HinhAnhRepository : IRepository<Hinhanh>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public async Task<bool> Add(Hinhanh entity)
        {
            try
            {
                await ContextDB.Hinhanhs.AddAsync(entity);
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
            string newMaImg = String.Empty;
            var lastMaImg = await ContextDB.Hinhanhs.OrderByDescending(sp => sp.MaHinhAnh).Select(sp => sp.MaHinhAnh).FirstOrDefaultAsync();
            if (lastMaImg == null)
                newMaImg = "img0001";
            else
                newMaImg = "img" + (int.Parse(lastMaImg.ToString().Substring(3)) + 1).ToString("D4");
            return newMaImg;
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Hinhanh>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Hinhanh entity)
        {
            throw new NotImplementedException();
        }
    }
}
