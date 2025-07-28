using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Twilio.TwiML.Voice;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NguoiDungRepository : IRepository<Nguoidung>
    {
        WebBanThuocBvtvContext _contextDB;

        public NguoiDungRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }

        public async Task<AlertMessage> Add(Nguoidung entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            if (await EmailIsExist(entity.Email))
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Tài khoản đã tồn tại";
            }
            else
            {
                try
                {
                    //Mã hóa password
                    entity.PassWord = BCrypt.Net.BCrypt.HashPassword(entity.PassWord);

                    await _contextDB.Nguoidungs.AddAsync(entity);
                    await _contextDB.SaveChangesAsync();
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
            return await _contextDB.Nguoidungs.AnyAsync(user => user.Email == email);
        }
        public async Task<string> CreateId()
        {
            string newMaNd = String.Empty;
            var lastMaNd = await _contextDB.Nguoidungs.OrderByDescending(user => user.MaNd).Select(user => user.MaNd).FirstOrDefaultAsync();
            if (lastMaNd == null)
                newMaNd = "nd000001";
            else
                newMaNd = "nd" + (int.Parse(lastMaNd.ToString().Substring(2)) + 1).ToString("D6");
            return newMaNd;
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Nguoidung>> GetAllCustomer()
        {
            return await _contextDB.Nguoidungs.Where(nd => nd.MaVaiTro == "KH").ToListAsync();
        }
        public Task<List<Nguoidung>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<AlertMessage> Update(Nguoidung entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.Nguoidungs.Update(entity);

                await _contextDB.SaveChangesAsync();

                alertMessage.Type = "success";
                alertMessage.Message = "Cập nhật thành công";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }
        public async Task<Nguoidung> UpdateEmail(string id, string email)
        {
            Nguoidung nguoidung = await _contextDB.Nguoidungs.Where(user => user.MaNd == id).FirstOrDefaultAsync();
            nguoidung.Email = email;
            _contextDB.Nguoidungs.Update(nguoidung);
            await _contextDB.SaveChangesAsync();
            return nguoidung;
        }
        public async Task<Nguoidung> Login(string email, string password)
        {
            Nguoidung nguoidung = await _contextDB.Nguoidungs.Where(user => user.Email == email).FirstOrDefaultAsync();
            if (nguoidung == null || !BCrypt.Net.BCrypt.Verify(password, nguoidung.PassWord))
            {
                return null;
            }

            return nguoidung;
        }
        public async Task<AlertMessage> LoginWithGoogle(Nguoidung nguoidung)
        {
            AlertMessage alertMessage = new AlertMessage();
            if (await EmailIsExist(nguoidung.Email))
            {
                Nguoidung user = await _contextDB.Nguoidungs.Where(user => user.Email == nguoidung.Email).FirstAsync();
                if (user.GoogleId == null) //đã đăng ký tài khoản email
                {
                    alertMessage.Type = "error";
                    alertMessage.Message = "Email đã đăng ký tài khoản trước đó";
                }
                else
                {
                    if (user.GoogleId == nguoidung.GoogleId)//Đăng nhập
                    {
                        alertMessage.Type = "success";
                        alertMessage.Message = "Đăng nhập tài khoản thành công";
                    }
                    else
                    {

                    }
                }
            }
            else//Đăng ký tài khoản
            {
                await _contextDB.Nguoidungs.AddAsync(nguoidung);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Đăng ký tài khoản thành công";
            }
            return alertMessage;
        }
        public async Task<Nguoidung> GetById(string id)
        {
            Nguoidung nguoidung = await _contextDB.Nguoidungs.FindAsync(id);
            return nguoidung;
        }
        public async Task<Nguoidung> GetByEmail(string email)
        {
            Nguoidung nguoidung = await _contextDB.Nguoidungs.Where(user => user.Email == email).FirstAsync();
            return nguoidung;
        }

        public async Task<AlertMessage> ChangePass(string email, string oldPass, string newPass)
        {
            AlertMessage alertMessage = new AlertMessage();
            Nguoidung user = await Login(email, oldPass);
            if (user == null)
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Mật khẩu cũ không đúng";
                return alertMessage;
            }
            else
            {
                try
                {
                    user.PassWord = BCrypt.Net.BCrypt.HashPassword(newPass);
                    _contextDB.Nguoidungs.Update(user);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Đổi mật khẩu thành công";
                }
                catch (Exception ex)
                {
                    alertMessage.Type = "error";
                    alertMessage.Message = ex.Message;
                }
                return alertMessage;
            }
        }
        public async Task<List<Nguoidung>> SearchNguoiDung(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Nguoidung>();

            List<Nguoidung> lstNd = await _contextDB.Nguoidungs
                                 .Where(sp => sp.HoTen.Contains(keyword))
                                 .ToListAsync();
            return lstNd;
        }
        public async Task<List<Nguoidung>> FilterCustomer(string name, GenderOptions gioiTinh, CreateAtOptions ngayTao, SortOptionsCustomer sort)
        {
            IQueryable<Nguoidung> query = _contextDB.Nguoidungs
                                  .Where(nd => nd.HoTen.Contains(name) && nd.MaVaiTro=="KH");

            switch (gioiTinh)
            {
                case GenderOptions.Nam:
                    query = query.Where(nd => nd.GioiTinh == true);
                    break;
                case GenderOptions.Nu:
                    query = query.Where(nd => nd.GioiTinh == false);
                    break;
                case GenderOptions.All:
                default:
                    break;
            }
            switch(ngayTao)
            {
                case CreateAtOptions.Week:
                    query = query.Where(nd => (nd.NgayTao >= DateTime.Now.AddDays(-7)));
                    break;
                case CreateAtOptions.Month:
                    query = query.Where(nd => (nd.NgayTao >= DateTime.Now.AddMonths(-1)));
                    break;
                case CreateAtOptions.Year:
                    query = query.Where(nd => (nd.NgayTao >= DateTime.Now.AddYears(-1)));
                    break;
                case CreateAtOptions.All:
                default:
                    break;
            }    
            switch (sort)
            {
                case SortOptionsCustomer.IdAsc:
                    query = query.OrderBy(nd => nd.MaNd);
                    break;
                case SortOptionsCustomer.IdDesc:
                    query = query.OrderByDescending(nd => nd.MaNd);
                    break;
                case SortOptionsCustomer.NameA_Z:
                    query = query.OrderBy(nd => nd.HoTen);
                    break;
                case SortOptionsCustomer.NameZ_A:
                    query = query.OrderByDescending(nd => nd.HoTen);
                    break;
                case SortOptionsCustomer.DateAsc:
                    query = query.OrderBy(nd => nd.NgayTao);
                    break;
                case SortOptionsCustomer.DateDesc:
                    query = query.OrderByDescending(nd => nd.NgayTao);
                    break;
                case SortOptionsCustomer.AgeAsc:
                    query = query.OrderByDescending(nd => nd.NgaySinh);
                    break;
                case SortOptionsCustomer.AgeDesc:
                    query = query.OrderBy(nd => nd.NgaySinh);
                    break;
                default:
                    query = query.OrderBy(nd => nd.MaNd);
                    break;
            }

            return await query.ToListAsync();
        }
    }
}
