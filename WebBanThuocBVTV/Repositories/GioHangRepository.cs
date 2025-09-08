using Microsoft.EntityFrameworkCore;
using WebBanThuocBVTV.Areas.Customer.Controllers;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class GioHangRepository : IRepository<Giohang>
    {
        WebBanThuocBvtvContext _contextDB;
        private readonly IdGeneratorHelper _idGeneratorHelper = new IdGeneratorHelper();

        public GioHangRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public string CreateId()
        {
            try
            {
                string newMaGh = _idGeneratorHelper.GenerateOrderCode();
                return newMaGh;
            }
            catch (Exception ex) {
                throw ex;

            }
        }
        public async Task<AlertMessage> Add(Giohang entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.Giohangs.Add(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Tạo giỏ hàng thành công";
                return alertMessage;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public async Task<AlertMessage> AddSanPham(GiohangSanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                GiohangSanpham gioHangSp = await _contextDB.GiohangSanphams.FirstOrDefaultAsync(ghsp => ghsp.MaSanPham == entity.MaSanPham && ghsp.MaGioHang == entity.MaGioHang);
                if(gioHangSp==null)
                {
                    await _contextDB.GiohangSanphams.AddAsync(entity);
                }else
                {
                    double gia = gioHangSp.TongTien / gioHangSp.SoLuong;
                    gioHangSp.SoLuong += entity.SoLuong;
                    gioHangSp.TongTien = gioHangSp.SoLuong * gia;
                    _contextDB.GiohangSanphams.Update(gioHangSp);
                }    

                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm vào giỏ hàng thành công";
                return alertMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
        }
        
        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<AlertMessage> Delete(GiohangSanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.GiohangSanphams.Remove(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm vào giỏ hàng thành công";
                return alertMessage;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }
        public Task<List<Giohang>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<Giohang> GetById(string maNd)
        {
            try
            {
                Giohang gioHang = await _contextDB.Giohangs.Where(gh => gh.MaNd == maNd).Include(gh => gh.GiohangSanphams).ThenInclude(ghsp => ghsp.MaSanPhamNavigation).ThenInclude(sp => sp.Hinhanhs).FirstOrDefaultAsync();
                return gioHang;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<GiohangSanpham> GetGioHangSanPham(string maGioHang, string maSp)
        {
            try
            {
                GiohangSanpham gioHangSp = await _contextDB.GiohangSanphams.Where(gh => gh.MaGioHang == maGioHang && gh.MaSanPham == maSp).FirstOrDefaultAsync();
                return gioHangSp;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<AlertMessage> Update(Giohang entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.Giohangs.Update(entity);
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
        public async Task<AlertMessage> UpdateProduct(GiohangSanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.GiohangSanphams.Update(entity);
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
    }
}
