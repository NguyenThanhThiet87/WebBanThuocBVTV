using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhomSanPhamRepository
    {
        private readonly WebBanThuocBvtvContext _contextDB;

        public NhomSanPhamRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public async Task<AlertMessage> Add(Nhomsanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                Nhomsanpham nsp = _contextDB.Nhomsanphams.Where(nsp => nsp.TenNhomSp == entity.TenNhomSp).FirstOrDefault();
                if(nsp==null)
                {
                    _contextDB.Add(entity);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Thêm thành công";
                    return alertMessage;
                }
                alertMessage.Type = "error";
                alertMessage.Message = "Thêm thất bại";
                return alertMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateId(string name)
        {
            string id = "";
            string[] split = name.Split(" ");
            for (int j = 0; j <= 2 && j < split.Length; j++)
            {
                id += (split[j])[0].ToString();
            }
            while (id.Length < 3) 
                id.Append('&');

            return id.ToUpper();
        }

        public async Task<AlertMessage> Delete(string id)
        {
            try
            {
                Nhomsanpham nsp = _contextDB.Nhomsanphams.Where(nsp => nsp.MaNhomSp == id).FirstOrDefault();

                if (nsp != null)
                {

                    _contextDB.Nhomsanphams.Remove(nsp);
                    await _contextDB.SaveChangesAsync();
                    return new AlertMessage()
                    {
                        Type = "success",
                        Message = "Xóa thành công"
                    };
                }
                else
                {
                    return new AlertMessage()
                    {
                        Type = "error",
                        Message = "Nhóm sản phẩm không tồn tại"
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Không thể xóa vì bản ghi đang được tham chiếu ở bảng khác.");
            }
            catch (Exception ex)
            {
                return new AlertMessage()
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }

        public async Task<List<Nhomsanpham>> GetAllAsync()
        {
            try
            {
                List<Nhomsanpham> lstNSp = await _contextDB.Nhomsanphams.ToListAsync();
                return lstNSp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Nhomsanpham> Get(string id)
        {
            try
            {
                Nhomsanpham nsp = await _contextDB.Nhomsanphams.Where(sp => sp.MaNhomSp == id).FirstOrDefaultAsync();
                return nsp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AlertMessage> Update(Nhomsanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.Update(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Cập nhật thành công";
                return alertMessage;
            }
            catch (Exception ex)
            {
                return new AlertMessage()
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }


        public async Task<List<Nhomsanpham>> Search(string key)
        {
            try
            {
                List<Nhomsanpham> lstNsp = _contextDB.Nhomsanphams.Where(nsp => nsp.TenNhomSp.ToLower().Contains(key.ToLower())).ToList();
                return lstNsp;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
