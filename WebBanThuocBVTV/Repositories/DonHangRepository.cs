using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories.Interfaces;

namespace WebBanThuocBVTV.Repositories
{
    public class DonHangRepository : IRepository<DonhangSanpham>
    {
        private readonly WebBanThuocBvtvContext _contextDB;
        private readonly IdGeneratorHelper _idGeneratorHelper = new IdGeneratorHelper();
        public DonHangRepository(WebBanThuocBvtvContext contextDB)
        {
            _contextDB = contextDB;
        }
        public async Task<AlertMessage> Add(List<DonhangSanpham> lsDhSp, Donhang donHang)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                await _contextDB.Donhangs.AddAsync(donHang);
                foreach (var dhSp in lsDhSp)
                {
                    await _contextDB.DonhangSanphams.AddAsync(dhSp);
                }
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Đặt hàng thành công";
            } catch(Exception ex)
            {
                alertMessage.Type = "warning";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public Task<AlertMessage> Add(DonhangSanpham entity)
        {
            throw new NotImplementedException();
        }

        public string CreateId()
        {
            string newMaDh = _idGeneratorHelper.GenerateOrderCode();
            return newMaDh;
        }

        public Task<AlertMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<DonhangSanpham>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Donhang Get(string id)
        {
            return _contextDB.Donhangs
                .Include(dh => dh.MaNdNavigation)
                .Include(dh=>dh.MaTrangThaiNavigation)
                .Include(dh=>dh.DonhangSanphams)
                .ThenInclude(dhsp=>dhsp.MaSanPhamNavigation)
                .ThenInclude(sp=>sp.Hinhanhs)
                .FirstOrDefault(dh => dh.MaDonHang == id);
        }
        public async Task<List<Donhang>> GetOrderHistory(string maNd)
        {
            List<Donhang> lst = await _contextDB.Donhangs.Where(dh => dh.MaNd == maNd).Include(dh=>dh.DonhangSanphams).ThenInclude(dhsp=>dhsp.MaSanPhamNavigation).Include(dh=>dh.MaTrangThaiNavigation).OrderByDescending(dh=>dh.NgayLap).ToListAsync();
            return lst;
        }
        public Task<AlertMessage> Update(DonhangSanpham entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Donhang>> FilterOrder(string id, string state, SortOptionsOrder sortOption)
        {
            IQueryable<Donhang> query = _contextDB.Donhangs
                                  .Where(dh=> dh.MaDonHang.Contains(id) && dh.MaTrangThai.Contains(state))
                                  .Include(dh => dh.MaNdNavigation)
                                  .Include(dh => dh.DonhangSanphams)
                                  .Include(dh => dh.MaTrangThaiNavigation);
   
            switch (sortOption)
            {
                case SortOptionsOrder.PriceAsc:
                    query = query.OrderBy(dh => dh.TongTien);
                    break;
                case SortOptionsOrder.PriceDesc:
                    query = query.OrderByDescending(dh => dh.TongTien);
                    break;
                case SortOptionsOrder.DateAsc:
                    query = query.OrderByDescending(dh => dh.NgayLap);
                    break;
                case SortOptionsOrder.DateDesc:
                    query = query.OrderBy(dh => dh.NgayLap);
                    break;
                default:
                    query = query.OrderByDescending(dh => dh.NgayLap);
                    break;
            }    
            return await query.ToListAsync();
        }
        public async Task<AlertMessage> SendOrder(string maDh)
        {
            AlertMessage alertMessage = new AlertMessage();

            Donhang dh = _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh).FirstOrDefault();
            if(dh.MaTrangThai == "PCD")
            {
                dh.MaTrangThai = "INT";
                _contextDB.Update(dh);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Cập nhật thành công";
                
            }else
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Đơn hàng chưa xử lý";
            }
            return alertMessage;
        }
        public async Task<AlertMessage> TransferredOrder(string maDh)
        {
            AlertMessage alertMessage = new AlertMessage();

            Donhang dh = _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh).FirstOrDefault();
            if (dh.MaTrangThai == "INT")
            {
                dh.MaTrangThai = "SHP";
                dh.NgayGiaoHang = DateTime.Now;
                _contextDB.Update(dh);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Cập nhật thành công";
            }
            else
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Đơn hàng chưa vận chuyển";
            }
            return alertMessage;
        }
        public async Task<AlertMessage> DeleteOrder(string maDh)
        {
            AlertMessage alertMessage = new AlertMessage();

            Donhang dh = _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh)
                                            .Include(dh => dh.DonhangSanphams)
                                            .FirstOrDefault();
            if(dh.MaPhuongThucTt!="NH")
            {
                Giaodich gd = _contextDB.Giaodiches.Where(gd => gd.MaDonHang == dh.MaDonHang).FirstOrDefault();
                _contextDB.Remove(gd);
            }    
            if (dh.MaTrangThai == "CMP" || dh.MaTrangThai=="PCD")
            {          
                _contextDB.RemoveRange(dh.DonhangSanphams);
                _contextDB.Remove(dh);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Xóa thành công";
            }
            else
            {
                alertMessage.Type = "warning";
                alertMessage.Message = "Đơn hàng chưa hoàn thành";
            }
            return alertMessage;
        }
        public Dictionary<string, int> Statistic()
        {
            Dictionary<string, int> statistic = new Dictionary<string, int>();

            int dhCurrentMonth = _contextDB.Donhangs.Where(dh => dh.NgayLap.Month == DateTime.Now.Month
                                                     && dh.NgayLap.Year == DateTime.Now.Year)
                                                    .Count();
            int dhPrevMonth = _contextDB.Donhangs.Where(dh => dh.NgayLap.Month == DateTime.Now.AddMonths(-1).Month
                                                     && dh.NgayLap.Year == DateTime.Now.Year)
                                                    .Count();

            int percent = ((dhCurrentMonth - dhPrevMonth) / (dhPrevMonth == 0 ? 1 : dhPrevMonth)) * 100;

            int revenueCurrentMonth = (int)  _contextDB.Donhangs.Where(dh => dh.MaTrangThai == "CMP" 
                                                       && dh.NgayLap.Month == DateTime.Now.Month
                                                       && dh.NgayLap.Year == DateTime.Now.Year)
                                                .Sum(dh => dh.TongTien);
            int revenuePrevMonth = (int) _contextDB.Donhangs.Where(dh => dh.MaTrangThai == "CMP"
                                                       && dh.NgayLap.Month == DateTime.Now.AddMonths(-1).Month
                                                       && dh.NgayLap.Year == DateTime.Now.Year)
                                                .Sum(dh => dh.TongTien);

            int percentRevenue = ((revenueCurrentMonth - revenuePrevMonth) / (revenuePrevMonth == 0 ? 1 : revenuePrevMonth)) * 100;

            statistic.Add("dhCurrentMonth", dhCurrentMonth);
            statistic.Add("percent", percent);
            statistic.Add("revenueCurrentMonth", revenueCurrentMonth);
            statistic.Add("percentRevenue", percentRevenue);

            return statistic;
        }
        public async Task<List<Donhang>> GetNewOrders()
        {
            List<Donhang> lstDh = await _contextDB.Donhangs.Where(dh => dh.NgayLap >= DateTime.Now.AddDays(-7))
                .Include(dh => dh.MaTrangThaiNavigation)
                .Include(dh => dh.DonhangSanphams)
                .ThenInclude(dhsp => dhsp.MaSanPhamNavigation)
                .Include(dh => dh.MaNdNavigation)
                .ToListAsync();
            return lstDh;
        }
        public async Task<int> CountProcessingOrder()
        {
            return _contextDB.Donhangs.Where(dh => dh.MaTrangThai == "PCD").Count();
        }

        public async Task<List<Phuongthucthanhtoan>> GetAllPTThanhToan()
        {
            return await _contextDB.Phuongthucthanhtoans.ToListAsync();
        }
        public async Task<AlertMessage> CancelOrder(string maDh)
        {
            AlertMessage alertMessage = new AlertMessage();

            Donhang dh = _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh)
                                            .Include(dh => dh.DonhangSanphams)
                                            .FirstOrDefault();
            if (dh.MaTrangThai == "PCD" && dh.MaPhuongThucTt != "NH")
            {
                Giaodich gd = _contextDB.Giaodiches.Where(gd => gd.MaDonHang == dh.MaDonHang).FirstOrDefault();
                _contextDB.Remove(gd);
            }
            if (dh.MaTrangThai == "PCD" || (dh.MaPhuongThucTt != "NH" && dh.MaTrangThai == "UNP"))
            {
                _contextDB.RemoveRange(dh.DonhangSanphams);
                _contextDB.Remove(dh);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Hủy đơn thành công";
            }
            else
            {
                alertMessage.Type = "warning";
                alertMessage.Message = "Đơn hàng đang vận chuyển";
            }
            return alertMessage;
        }
        public async Task<Donhang> GetDonHangNotPayment(string maDh)
        {
            Donhang dh = await _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh && dh.MaPhuongThucTt != "NH").Include(dh => dh.MaNdNavigation).Include(dh => dh.MaPhuongThucTtNavigation).FirstOrDefaultAsync();
            return dh;
        }
        public async Task<AlertMessage> CompletingOrder(string maDh)
        {
            AlertMessage alertMessage = new AlertMessage();

            Donhang dh = _contextDB.Donhangs.Where(dh => dh.MaDonHang == maDh)
                                            .FirstOrDefault();
            if (dh.MaTrangThai == "SHP")
            {
                dh.MaTrangThai = "CMP";
                dh.NgayGiaoHang = DateTime.Now;
                _contextDB.Update(dh);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = "Xác nhận nhận hàng thành công";
            }
            else
            {
                alertMessage.Type = "error";
                alertMessage.Message = "Đơn hàng chưa vận chuyển";
            }
            return alertMessage;
        }

        public async Task<Dictionary<DateTime,double>> RevenueClostSixMonth()
        {
            DateTime today = DateTime.Today;
            DateTime dateMin = new DateTime(today.Year, today.Month - 6, 1);

            Dictionary<DateTime, double> lst = await _contextDB.Donhangs.Where(dh => dh.MaTrangThai == "CMP" && dh.NgayGiaoHang!=null && dh.NgayGiaoHang.Value > dateMin)
                .GroupBy(dh => new {dh.NgayGiaoHang.Value.Month, dh.NgayGiaoHang.Value.Year})
                .Select(g => new {g.Key.Month, g.Key.Year, Total = g.Sum(dh => (double)(dh.TongTien))})
                .ToDictionaryAsync(x => new DateTime(x.Year,x.Month,1), x=>x.Total);
            
            return lst;
        }
    }
}
