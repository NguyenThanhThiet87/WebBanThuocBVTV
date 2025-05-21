using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Binhluan
{
    public DateTime ThoiGian { get; set; }

    public string MaNd { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public int MaDanhGia { get; set; }

    public virtual Danhgium MaDanhGiaNavigation { get; set; } = null!;

    public virtual Nguoidung MaNdNavigation { get; set; } = null!;

    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
