using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class BinhLuanRepository : IRepository<Binhluan>
    {
        WebBanThuocBvtvContext _contextDB;

        public BinhLuanRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }

        public async Task<AlertMessage> Add(Binhluan entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                await _contextDB.AddAsync(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = $"Bạn vừa bình luận sản phẩm";
            }catch(Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public Task<string> CreateId()
        {
            throw new NotImplementedException();
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Binhluan>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AlertMessage> Update(Binhluan entity)
        {
            throw new NotImplementedException();
        }
    }
}
