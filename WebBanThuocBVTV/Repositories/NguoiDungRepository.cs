using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NguoiDungRepository : IRepository<Nguoidung>
    {
        WebBanThuocBvtvContext ContextDB = new WebBanThuocBvtvContext();
        public async Task<AlertMessage> Add(Nguoidung entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            if (await EmailIsExist(entity.Email))
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Tài khoản đã tồn tại";
            }else
            {
                try
                {
                    //Mã hóa password
                    entity.PassWord = BCrypt.Net.BCrypt.HashPassword(entity.PassWord);

                    await ContextDB.Nguoidungs.AddAsync(entity);
                    await ContextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Tạo tài khoản thành công";
                }
                catch (Exception ex)
                {
                    alertMessage.Type = "error";
                    alertMessage.Message = ex.Message;
                }
            }    
                
            return alertMessage;
        }
        public async Task<bool> EmailIsExist(string email)
        {
            return await ContextDB.Nguoidungs.AnyAsync(user => user.Email == email);
        }
        public async Task<string> CreateId()
        {
            string newMaNd = String.Empty;
            var lastMaNd = await ContextDB.Nguoidungs.OrderByDescending(user => user.MaNd).Select(user => user.MaNd).FirstOrDefaultAsync();
            if (lastMaNd == null)
                newMaNd = "nd000001";
            else
                newMaNd = "nd" + (int.Parse(lastMaNd.ToString().Substring(2)) + 1).ToString("D6");
            return newMaNd;
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Nguoidung>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Nguoidung entity)
        {
            throw new NotImplementedException();
        }
        public async Task<Nguoidung> Login(string email, string password)
        {
            Nguoidung nguoidung;
            nguoidung = await ContextDB.Nguoidungs.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (nguoidung == null || !BCrypt.Net.BCrypt.Verify(password, nguoidung.PassWord))
            {
                return null;
            }
            
            return nguoidung;
        }

    }
}
