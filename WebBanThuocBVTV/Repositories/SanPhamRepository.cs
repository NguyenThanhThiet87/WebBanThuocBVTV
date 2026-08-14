using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Threading.Tasks;
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
                if (entity.Hinhanhs.Count > 0)
                {
                    Hinhanh img = entity.Hinhanhs.First();

                    img.MaHinhAnh = await CreateIdImg();
                    await _contextDB.Hinhanhs.AddAsync(img);
                }
                await _contextDB.Sanphams.AddAsync(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Thêm thành công";

                return alertMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> CreateIdImg()
        {
            try
            {
                string newMaImg = String.Empty;
                var lastMaImg = await _contextDB.Hinhanhs.OrderByDescending(img => img.MaHinhAnh).Select(img => img.MaHinhAnh).FirstOrDefaultAsync();
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

        public async Task<string> CreateId()
        {
            try
            {
                string newMaSp = String.Empty;
                var lastMaSp = await _contextDB.Sanphams.OrderByDescending(sp => sp.MaSanPham).Select(sp => sp.MaSanPham).FirstOrDefaultAsync();
                if (lastMaSp == null)
                    newMaSp = "sp0001";
                else
                    newMaSp = "sp" + (int.Parse(lastMaSp.ToString().Substring(2)) + 1).ToString("D4");
                return newMaSp;
            }catch (Exception ex) { throw ex; }
        }

        public async Task<AlertMessage> Discontinue(string id) //ngừng kinh doanh sản phẩm
        {
            try
            {
                AlertMessage alertMessage = new AlertMessage();
                Sanpham sp = await _contextDB.Sanphams.Where(sp => sp.MaSanPham == id).FirstOrDefaultAsync();
                if (sp.IsActive)
                {
                    sp.IsActive = false;
                    _contextDB.Update(sp);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Ngừng kinh doanh thành công";
                }
                else
                {
                    alertMessage.Type = "warning";
                    alertMessage.Message = "Sản phẩm đã ngừng kinh doanh";
                }
                return alertMessage;
            }catch(Exception ex) { throw ex; }
        }
        public async Task<AlertMessage> Sell(string id) //kinh doanh sản phẩm
        {
            try
            {
                AlertMessage alertMessage = new AlertMessage();
                Sanpham sp = await _contextDB.Sanphams.Where(sp => sp.MaSanPham == id).FirstOrDefaultAsync();
                if (!sp.IsActive)
                {
                    sp.IsActive = true;
                    _contextDB.Update(sp);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "kinh doanh sản phẩm thành công";
                }
                else
                {
                    alertMessage.Type = "warning";
                    alertMessage.Message = "Sản phẩm đang kinh doanh";
                }
                return alertMessage;
            }catch(Exception ex) { throw ex; }
        }
        public async Task<List<Sanpham>> GetAllAsync()
        {
            try
            {
                List<Sanpham> lstSp = await _contextDB.Sanphams
                                .Include(sp => sp.Hinhanhs)
                                .Include(sp => sp.Binhluans)
                                .Include(sp => sp.MaNhaSxNavigation)
                                .Include(sp => sp.MaNhomSpNavigation)
                                .ToListAsync();
                return lstSp;
            }catch (Exception ex) { throw ex; }
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
                              .Include(sp => sp.Binhluans)
                              .ThenInclude(bl => bl.Phanhois)
                              .ThenInclude(ph => ph.MaNhanVienNavigation)
                              .FirstAsync();
                return sp;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Sanpham> GetByIdBase(string maSp)
        {
            try
            {
                Sanpham? sp = await _contextDB.Sanphams
                              .Where(sp => sp.MaSanPham == maSp)
                              .FirstAsync();
                return sp;
            }
            catch (Exception ex)
            {
                throw ex;
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

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> Count()
        {
            try
            {
                int count = await _contextDB.Sanphams.CountAsync();
                return count;
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<Sanpham>> FeatureProduct()
        {
            try
            {
                List<Sanpham> lstSp = await _contextDB.Sanphams
                .GroupJoin(_contextDB.DonhangSanphams.Where(dhsp => dhsp.MaDonHangNavigation.MaTrangThai == "CMP"),
                                                                      sp => sp.MaSanPham,
                                                                      dhsp => dhsp.MaSanPham,
                                                                      (sp, group) => new { sanPham = sp, tongMua = group.Sum(gp => gp.SoLuongDatMua) })
                .OrderByDescending(sp => sp.tongMua).Select(sp => sp.sanPham)
                .Include(sp => sp.Hinhanhs)
                .ToListAsync();

                return lstSp.GetRange(0, 4);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<List<Sanpham>> FilterProduct(string name = "", bool isActive = true, string maNhomSp = "", string maNhaSx = "", PriceArrange? priceArrange = null, QuantityOptions? quantityOption = null, SortOptions? sort = null, SortPrice? sortPrice = null)
        {
            try
            {
                IQueryable<Sanpham> query = _contextDB.Sanphams
                                  .Where(sp => sp.TenSanPham.Contains(name)
                                                         && sp.MaNhaSx.Contains(maNhaSx)
                                                         && sp.MaNhomSp.Contains(maNhomSp)
                                                         && sp.IsActive == isActive)
                                  .Include(sp => sp.MaNhomSpNavigation)
                                  .Include(sp => sp.MaNhaSxNavigation)
                                  .Include(sp => sp.Hinhanhs)
                                  .Include(sp => sp.Binhluans)
                                  .Include(sp => sp.DonhangSanphams)
                                  .ThenInclude(dhsp => dhsp.MaDonHangNavigation);

                switch (priceArrange)
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

                switch (quantityOption)
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
                switch (sortPrice)
                {
                    case SortPrice.priceAsc:
                        query = query.OrderBy(sp => sp.Gia);
                        break;
                    case SortPrice.priceDesc:
                        query = query.OrderByDescending(sp => sp.Gia);
                        break;
                    default:
                        break;
                }
                return await query.ToListAsync();
            }catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AlertMessage> Delete(string id)
        {
            try
            {
                Sanpham sp = await _contextDB.Sanphams.Include(sp => sp.Hinhanhs).Include(sp => sp.DonhangSanphams).Include(sp => sp.GiohangSanphams).FirstOrDefaultAsync(sp => sp.MaSanPham == id);
                if (sp != null)
                {
                    if(!sp.IsActive && !sp.DonhangSanphams.Any())
                    {
                        var imgs = sp.Hinhanhs;
                        if (imgs != null)
                        {
                            _contextDB.Hinhanhs.RemoveRange(imgs); 
                        }
                        var spGh = sp.GiohangSanphams;
                        if (spGh != null)
                        {
                            _contextDB.GiohangSanphams.RemoveRange(spGh); 
                        }
                        _contextDB.Sanphams.Remove(sp);
                        await _contextDB.SaveChangesAsync();
                        return new AlertMessage()
                        {
                            Type = "success",
                            Message = "Xóa thành công"
                        };
                    } else
                    {
                        return new AlertMessage()
                        {
                            Type = "error",
                            Message = "Xóa thất bại, sản phẩm đang được tham chiếu"
                        };
                    }
                }
                else
                {
                    return new AlertMessage()
                    {
                        Type = "error",
                        Message = "Sản phẩm không tồn tại"
                    };
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Không thể xóa vì bản ghi đang được tham chiếu ở bảng khác.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Dictionary<string, int> Statistic()
        {
            try
            {
                Dictionary<string, int> statistic = new Dictionary<string, int>();

                int spCurrent = _contextDB.Sanphams.Where(sp => sp.IsActive).Count();
                int spNoneActive = _contextDB.Sanphams.Where(sp => !sp.IsActive).Count();
                statistic.Add("Count", spCurrent);
                statistic.Add("CountNoneActive", spNoneActive);

                return statistic;
            }
            catch (Exception ex) {
                throw ex;

            }
        }
        public async Task<List<Sanpham>> GetOutOfStockProduct()
        {
            try
            {
                return await _contextDB.Sanphams.Where(sp => sp.SoLuong < 10)
                                     .Include(sp => sp.MaNhomSpNavigation)
                                     .ToListAsync();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<AlertMessage> SellProduct(List<Dictionary<string, int>> lstSp)
        {
            try
            {
                AlertMessage alertMessage = new AlertMessage();
                List<Sanpham> lst = new List<Sanpham>();
                foreach (var item in lstSp)
                {
                    string maSp = item.Keys.First();
                    int soLuong = item.Values.First();
                    Sanpham sp = await _contextDB.Sanphams.Where(sp => sp.MaSanPham == maSp).FirstOrDefaultAsync();
                    if (sp != null)
                    {
                        if (sp.SoLuong >= soLuong)
                        {
                            sp.SoLuong -= soLuong;
                            lst.Add(sp);
                        }
                        else
                        {
                            alertMessage.Type = "warning";
                            alertMessage.Message = $"Sản phẩm {sp.TenSanPham} không đủ hàng";
                            return alertMessage;
                        }
                    }
                    else
                    {
                        alertMessage.Type = "error";
                        alertMessage.Message = $"Sản phẩm {maSp} không tồn tại";
                        return alertMessage;
                    }
                }
                foreach (var sp in lst)
                {
                    _contextDB.Update(sp);
                    await _contextDB.SaveChangesAsync();
                    alertMessage.Type = "success";
                    alertMessage.Message = "Mua hàng thành công";
                }
                return alertMessage;
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<Sanpham>> SuggestProduct(string keyword)
        {
            try
            {
                return await _contextDB.Sanphams.Where(sp => sp.CongDung.ToLower().Contains(keyword.ToLower())).Include(sp => sp.Hinhanhs).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
