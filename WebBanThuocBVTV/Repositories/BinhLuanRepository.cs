using Microsoft.EntityFrameworkCore;
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
                await _contextDB.Binhluans.AddAsync(entity);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = $"Bạn vừa bình luận sản phẩm";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public Task<string> CreateId()
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<AlertMessage> Delete(string id)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Binhluan> GetById(DateTime thoiGian, string maNd, string maSanPham)
        {
            try
            {
                Binhluan comment = await _contextDB.Binhluans.Where(bl => bl.ThoiGian.Date == thoiGian.Date
                                                  && bl.ThoiGian.Hour == thoiGian.Hour
                                                  && bl.ThoiGian.Minute == thoiGian.Minute
                                                  && bl.ThoiGian.Second == thoiGian.Second
                                                  && bl.MaNd == maNd
                                                  && bl.MaSanPham == maSanPham)
                                           .Include(bl => bl.MaNdNavigation)
                                           .Include(bl => bl.MaSanPhamNavigation)
                                           .FirstOrDefaultAsync();
                return comment;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<List<Binhluan>> GetAllAsync()
        {
            try
            {
                return _contextDB.Binhluans
                                 .Include(bl => bl.MaNdNavigation)
                                 .Include(bl => bl.MaSanPhamNavigation)
                                 .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<AlertMessage> Update(Binhluan entity)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Binhluan>> FilterComment(string name, EvaluateOptions? evaluateOptions = null, IsReply? isReplyOptions = null, StateComment? stateOptions = null)
        {
            try
            {
                IQueryable<Binhluan> query = _contextDB.Binhluans
                                      .Where(bl => bl.MaNdNavigation.HoTen.Contains(name))
                                      .Include(bl => bl.MaNdNavigation)
                                      .Include(bl => bl.MaSanPhamNavigation)
                                      .Include(bl => bl.Phanhois)
                                      .ThenInclude(ph => ph.MaNhanVienNavigation);

                switch (evaluateOptions)
                {
                    case EvaluateOptions.one:
                        query = query.Where(bl => bl.MaDanhGia == 1);
                        break;
                    case EvaluateOptions.two:
                        query = query.Where(bl => bl.MaDanhGia == 2);
                        break;
                    case EvaluateOptions.three:
                        query = query.Where(bl => bl.MaDanhGia == 3);
                        break;
                    case EvaluateOptions.four:
                        query = query.Where(bl => bl.MaDanhGia == 4);
                        break;
                    case EvaluateOptions.five:
                        query = query.Where(bl => bl.MaDanhGia == 5);
                        break;
                    default:
                        break;
                }
                switch (isReplyOptions)
                {
                    case IsReply.none:
                        query = query.Where(bl => bl.Phanhois.Count == 0);
                        break;
                    case IsReply.done:
                        query = query.Where(bl => bl.Phanhois.Count > 0);
                        break;
                    default:
                        break;
                }
                switch (stateOptions)
                {
                    case StateComment.newComment:
                        query = query.OrderByDescending(bl => bl.ThoiGian);
                        break;
                    case StateComment.oldComment:
                        query = query.OrderBy(bl => bl.ThoiGian);
                        break;
                    default:
                        break;
                }

                return await query.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<AlertMessage> Reply(Phanhoi ph)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                await _contextDB.AddAsync(ph);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = $"Phản hồi đã được gửi";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public async Task<AlertMessage> DeleteReply(int maPh)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                Phanhoi ph = _contextDB.Phanhois.Where(ph => ph.MaPhanHoi == maPh).FirstOrDefault();
                _contextDB.Phanhois.Remove(ph);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = $"Phản hồi đã được xóa";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public async Task<AlertMessage> EditReply(int maPh, string maNv, string noiDung)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                Phanhoi ph = await _contextDB.Phanhois.Where(ph => ph.MaPhanHoi == maPh).FirstOrDefaultAsync();
                ph.NoiDungPhanHoi = noiDung;
                ph.MaNhanVien = maNv;

                _contextDB.Phanhois.Update(ph);
                await _contextDB.SaveChangesAsync();
                alertMessage.Type = "success";
                alertMessage.Message = $"Phản hồi đã được cập nhật";
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }

        public async Task<Phanhoi> GetReplyById(int maPh)
        {
            try
            {
                Phanhoi reply = await _contextDB.Phanhois.Where(bl => bl.MaPhanHoi == maPh)
                                           .Include(ph => ph.Binhluan)
                                           .ThenInclude(bl => bl.MaNdNavigation)
                                           .Include(ph => ph.Binhluan)
                                           .ThenInclude(bl => bl.MaSanPhamNavigation)
                                           .Include(ph => ph.MaNhanVienNavigation)
                                           .FirstOrDefaultAsync();
                return reply;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
