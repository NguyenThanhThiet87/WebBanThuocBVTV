using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class NhaSanXuatRepository
    {
        private readonly WebBanThuocBvtvContext _contextDB;

        public NhaSanXuatRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public async Task<AlertMessage> Add(Nhasanxuat entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                Nhasanxuat nsx = _contextDB.Nhasanxuats.Where(nsx => nsx.TenNhaSx == entity.TenNhaSx).FirstOrDefault();
                if (nsx == null)
                {
                    _contextDB.Add(entity);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Thêm thành công";
                    return alertMessage;
                }
                alertMessage.Type = "warning";
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
                Nhasanxuat nsx = _contextDB.Nhasanxuats.Where(nsp => nsp.MaNhaSx == id).FirstOrDefault();

                if (nsx != null)
                {

                    _contextDB.Nhasanxuats.Remove(nsx);
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

        public async Task<List<Nhasanxuat>> GetAllAsync()
        {
            try
            {
                List<Nhasanxuat> listNsx = await _contextDB.Nhasanxuats.ToListAsync();
                return listNsx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Nhasanxuat> Get(string id)
        {
            try
            {
                Nhasanxuat nsx = await _contextDB.Nhasanxuats.Where(sp => sp.MaNhaSx == id).FirstOrDefaultAsync();
                return nsx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AlertMessage> Update(Nhasanxuat entity)
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
                throw ex;
            }
        }
        public async Task<List<Nhasanxuat>> Search(string key)
        {
            try
            {
                List<Nhasanxuat> lstNsp = _contextDB.Nhasanxuats.Where(nsp => nsp.TenNhaSx.ToLower().Contains(key.ToLower())).ToList();
                return lstNsp;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
