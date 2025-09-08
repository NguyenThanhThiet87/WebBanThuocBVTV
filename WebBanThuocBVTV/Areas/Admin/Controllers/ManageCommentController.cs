using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;
using X.PagedList.Extensions;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageCommentController : BaseController
    {
        private BinhLuanRepository _binhLuanRepository;

        public ManageCommentController(BinhLuanRepository binhLuanRepository)
        {
            _binhLuanRepository = binhLuanRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                SavePointSideBar(SideBar.BinhLuan);

                List<Binhluan> lstBl = await _binhLuanRepository.GetAllAsync();
                return View(lstBl);
            }
            catch (Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ReplyComment(Binhluan bl)
        {
            try
            {
                Binhluan comment = await _binhLuanRepository.GetById(bl.ThoiGian, bl.MaNd, bl.MaSanPham);

                return PartialView("_ReplyComment", comment);
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> Reply(Phanhoi ph)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                ph.NgayPhanHoi = DateTime.Now;

                alertMessage = await _binhLuanRepository.Reply(ph);
            }
            catch (Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }
        [HttpPost]
        public async Task<IActionResult> EditReplyComment(string maPh)
        {
            try
            {
                Phanhoi reply = await _binhLuanRepository.GetReplyById(int.Parse(maPh));

                return PartialView("_EditReply", reply);
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> EditReply(string maPh, string maNv, string noiDung)
        {
            AlertMessage alertMessage = new AlertMessage();
            try
            {
                alertMessage = await _binhLuanRepository.EditReply(int.Parse(maPh), maNv, noiDung);
            }catch(Exception ex)
            {
                alertMessage.Type = "error";
                alertMessage.Message = ex.Message;
            }
            return alertMessage;
        }
        [HttpPost]
        public async Task<IActionResult> FilterComment(string keyword="", EvaluateOptions? evaluateOptions=null, IsReply? isReplyOptions = null, StateComment? stateOptions=null, int? page=1)
        {
            try
            {
                keyword = keyword ?? "";

                if (page == null)
                    page = 1;

                List<Binhluan> lstComments = await _binhLuanRepository.FilterComment(keyword, evaluateOptions, isReplyOptions, stateOptions);

                int pageSize = 12; // Số sản phẩm hiển thị trên mỗi trang

                int pageNumber = page ?? 1;

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = lstComments.Count / pageSize;

                return PartialView("_ListComment", lstComments.ToPagedList(pageNumber, pageSize));
            }catch(Exception ex)
            {
                SetAlert($"Xảy ra lỗi: {ex.Message}", "error");
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public async Task<AlertMessage> DeleteReply(string maPh)
        {
            AlertMessage alert = new AlertMessage();
            try
            {
                alert = await _binhLuanRepository.DeleteReply(int.Parse(maPh));

            }catch(Exception ex)
            {
                alert.Type = "error";
                alert.Message = ex.Message;
            }
            return alert;
        }
    }
}
