using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebBanThuocBVTV.Helper;
using WebBanThuocBVTV.Models;
using WebBanThuocBVTV.Repositories;

namespace WebBanThuocBVTV.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ManageSystemController : BaseController
    {
        readonly NhomSanPhamRepository _nhomSanPhamRepository;
        readonly NhaSanXuatRepository _nhaSanXuatRepository;
        public ManageSystemController(NhomSanPhamRepository nhomSanPhamRepository, NhaSanXuatRepository nhaSanXuatRepository)
        {
            _nhomSanPhamRepository = nhomSanPhamRepository;
            _nhaSanXuatRepository = nhaSanXuatRepository;
        }

        public async Task<IActionResult> Index()
        {
            SavePointSideBar(SideBar.HeThong);

            ViewBag.NhomSanPham = await _nhomSanPhamRepository.GetAllAsync();
            ViewBag.NhaSanXuat = await _nhaSanXuatRepository.GetAllAsync();
            return View();
        }
        [HttpPost]
        public IActionResult AddCategoryModal()
        {
            return PartialView("_AddCategory");
        }
        [HttpPost]
        public async Task<AlertMessage> AddCategory(string name)
        {
            try
            {
                string id = _nhomSanPhamRepository.CreateId(name);
                Nhomsanpham nsp = new Nhomsanpham()
                {
                    MaNhomSp = id,
                    TenNhomSp = name
                };
                AlertMessage alertMessage = await _nhomSanPhamRepository.Add(nsp);
                return alertMessage;
            }catch(Exception ex)
            {
                return new AlertMessage()
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCategoryModal(string id)
        {
            Nhomsanpham nsp = await _nhomSanPhamRepository.Get(id);
            return PartialView("_EditCategory", nsp);
        }
        [HttpPost]
        public async Task<AlertMessage> EditCategory(Nhomsanpham nsp)
        {
            AlertMessage alertMessage = await _nhomSanPhamRepository.Update(nsp);
            return alertMessage;
        }

        [HttpPost]
        public async Task<AlertMessage> DeleteCategory(string id)
        {
            try
            {
                AlertMessage alert = await _nhomSanPhamRepository.Delete(id);
                return alert;
            }catch(Exception ex)
            {
                return new AlertMessage()
                {
                    Type = "error",
                    Message = ex.Message
                };
            }
        }
        [HttpPost]
        public async Task<IActionResult> SearchCategory(string key)
        {
            key = key ?? "";
            List<Nhomsanpham> lstNsp = await _nhomSanPhamRepository.Search(key);
            return PartialView("_ListCategory", lstNsp);
        }
        //NSX
        [HttpPost]
        public IActionResult AddManuModal()
        {
            return PartialView("_AddManu");
        }
        [HttpPost]
        public async Task<AlertMessage> AddManu(string name)
        {
            try
            {
                string id = _nhaSanXuatRepository.CreateId(name);
                Nhasanxuat nsp = new Nhasanxuat()
                {
                    MaNhaSx = id,
                    TenNhaSx = name
                };
                AlertMessage alertMessage = await _nhaSanXuatRepository.Add(nsp);
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

        [HttpPost]
        public async Task<IActionResult> EditManuModal(string id)
        {
            Nhasanxuat nsx = await _nhaSanXuatRepository.Get(id);
            return PartialView("_EditManu", nsx);
        }
        [HttpPost]
        public async Task<AlertMessage> EditManu(Nhasanxuat nsx)
        {
            AlertMessage alertMessage = await _nhaSanXuatRepository.Update(nsx);
            return alertMessage;
        }

        [HttpPost]
        public async Task<AlertMessage> DeleteManu(string id)
        {
            try
            {
                AlertMessage alert = await _nhaSanXuatRepository.Delete(id);
                return alert;
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
        [HttpPost]
        public async Task<IActionResult> SearchManu(string key)
        {
            key = key ?? "";
            List<Nhasanxuat> lstNsx = await _nhaSanXuatRepository.Search(key);
            return PartialView("_ListManu", lstNsx);
        }
    }
}
