using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class HinhAnhRepository : IRepository<Hinhanh>
    {
        private readonly WebBanThuocBvtvContext ContextDB;

        public HinhAnhRepository(WebBanThuocBvtvContext contextDB)
        {
            ContextDB = contextDB;
        }

        public async Task<AlertMessage> Add(Hinhanh entity)
        {
            AlertMessage alertMessage = new AlertMessage();

            try
            {
                await ContextDB.Hinhanhs.AddAsync(entity);
                await ContextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm ảnh thành công";
                return alertMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<string> CreateId()
        {
            try
            {
                string newMaImg = String.Empty;
                var lastMaImg = await ContextDB.Hinhanhs.OrderByDescending(sp => sp.MaHinhAnh).Select(sp => sp.MaHinhAnh).FirstOrDefaultAsync();
                if (lastMaImg == null)
                    newMaImg = "img0001";
                else
                    newMaImg = "img" + (int.Parse(lastMaImg.ToString().Substring(3)) + 1).ToString("D4");
                return newMaImg;
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Hinhanh>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AlertMessage> Update(Hinhanh entity)
        {
            throw new NotImplementedException();
        }
    }
}
