using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class HinhAnhRepository : IRepository<Hinhanh>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();

        public async Task<AlertMessage> Add(Hinhanh entity)
        {
            AlertMessage alertMessage = new AlertMessage();

            try
            {
                await ContextDB.Hinhanhs.AddAsync(entity);
                await ContextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm ảnh thành công";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
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
