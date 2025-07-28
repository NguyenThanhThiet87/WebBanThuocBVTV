using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class SanPhamRepository : IRepository<Sanpham>
    {
        WebBanThuocBvtvContext _contextDB;

        public SanPhamRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }

        public async Task<AlertMessage> Add(Sanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                await _contextDB.Sanphams.AddAsync(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm thành công";
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
            string newMaSp = String.Empty;
            var lastMaSp = await _contextDB.Sanphams.OrderByDescending(sp => sp.MaSanPham).Select(sp => sp.MaSanPham).FirstOrDefaultAsync();
            if (lastMaSp == null)
                newMaSp = "sp0001";
            else
                newMaSp = "sp" + (int.Parse(lastMaSp.ToString().Substring(2)) + 1).ToString("D4");
            return newMaSp;
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Sanpham>> GetAllAsync()
        {
            List<Sanpham> lstSp = await _contextDB.Sanphams
                                 .Include(sp => sp.Hinhanhs)
                                 .Include(sp => sp.Binhluans)
                                 .Include(sp => sp.MaNhaSxNavigation)
                                 .Include(sp => sp.MaNhomSpNavigation)
                                 .ToListAsync();
            return lstSp;
        }
        public async Task<Sanpham> GetById(string maSp)
        {
            try
            {
                Sanpham? sp = await _contextDB.Sanphams
                              .Where(sp => sp.MaSanPham == maSp)
                              .Include(sp => sp.Hinhanhs)
                              .Include(sp => sp.MaNhomSpNavigation)
                              .Include(sp => sp.MaNhaSxNavigation)
                              .Include(sp => sp.Binhluans)
                              .ThenInclude(bl => bl.MaNdNavigation)
                              .FirstAsync();
                return sp;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<AlertMessage> Update(Sanpham entity)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                _contextDB.Update(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Cập nhật sản phẩm thành công";
                return alertMessage;

            }catch(Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
                return alertMessage;
            }
        }
        public async Task<int> Count()
        {
            int count = await _contextDB.Sanphams.CountAsync();
            return count;
        }
        //public async Task<List<Sanpham>> FilterProduct(string maNhomSp)
        //{
        //    List<Sanpham> lstSp = await _contextDB.Sanphams
        //                          .Where(sp => sp.MaNhomSp == maNhomSp)
        //                          .Include(sp => sp.Hinhanhs).Include(sp => sp.Binhluans)
        //                          .Include(sp => sp.DonhangSanphams)
        //                          .ThenInclude(dhsp => dhsp.MaDonHangNavigation)
        //                          .ToListAsync();
        //    return lstSp;
        //}

        public async Task<List<Sanpham>> FeatureProduct()
        {
            List<Sanpham> lstSp = await _contextDB.Sanphams
                .GroupJoin(_contextDB.DonhangSanphams.Where(dhsp => dhsp.MaDonHangNavigation.MaTrangThai == "HTO"), 
                                                                      sp => sp.MaSanPham,
                                                                      dhsp => dhsp.MaSanPham, 
                                                                      (sp, group) => new { sanPham = sp, tongMua = group.Sum(gp => gp.SoLuongDatMua) })
                .OrderByDescending(sp => sp.tongMua).Select(sp => sp.sanPham)
                .Include(sp=>sp.Hinhanhs)
                .ToListAsync();
            
            return lstSp.GetRange(0, 4);
        } 
        public async Task<List<Sanpham>> FilterProduct(string name="", string maNhomSp = "", string maNhaSx = "", PriceArrange? priceArrange=null, QuantityOptions? quantityOption = null, SortOptions sort=SortOptions.IdAsc)
        {
            IQueryable<Sanpham> query = _contextDB.Sanphams
                                  .Where(sp => sp.TenSanPham.Contains(name)
                                                         && sp.MaNhaSx.Contains(maNhaSx)
                                                         && sp.MaNhomSp.Contains(maNhomSp))
                                  .Include(sp => sp.MaNhomSpNavigation)
                                  .Include(sp => sp.MaNhaSxNavigation)
                                  .Include(sp => sp.Hinhanhs).Include(sp => sp.Binhluans)
                                  .Include(sp => sp.DonhangSanphams)
                                  .ThenInclude(dhsp => dhsp.MaDonHangNavigation);

            switch(priceArrange)
            {
                case PriceArrange.Bel150:
                    query = query.Where(sp => sp.Gia < 150000);
                    break;
                case PriceArrange.fr150t350:
                    query = query.Where(sp => (sp.Gia >= 150000 && sp.Gia <= 350000));
                    break;
                case PriceArrange.Abo350:
                    query = query.Where(sp => sp.Gia > 350000);
                    break;
                default:
                    break;
            }    

            switch(quantityOption)
            {
                case QuantityOptions.Avaiable:
                    query = query.Where(sp => sp.SoLuong >= 10);
                    break;
                case QuantityOptions.OutOfShock:
                    query = query.Where(sp => sp.SoLuong < 10);
                    break;
                default:
                    break;
            }    

            switch (sort)
            {
                case SortOptions.IdAsc:
                    query = query.OrderBy(sp => sp.MaSanPham);
                    break;
                case SortOptions.IdDesc:
                    query = query.OrderByDescending(sp => sp.MaSanPham);
                    break;
                case SortOptions.NameA_Z:
                    query = query.OrderBy(sp => sp.TenSanPham);
                    break;
                case SortOptions.NameZ_A:
                    query = query.OrderByDescending(sp => sp.TenSanPham);
                    break;
                case SortOptions.PriceAsc:
                    query = query.OrderBy(sp => sp.Gia);
                    break;
                case SortOptions.PriceDesc:
                    query = query.OrderByDescending(sp => sp.Gia);
                    break;
                case SortOptions.QuantityAsc:
                    query = query.OrderBy(sp => sp.SoLuong);
                    break;
                case SortOptions.QuantityDesc:
                    query = query.OrderByDescending(sp => sp.SoLuong);
                    break;
                default:
                    query = query.OrderBy(sp => sp.TenSanPham);
                    break;
            }

            return await query.ToListAsync();
        }
        
    }
}
