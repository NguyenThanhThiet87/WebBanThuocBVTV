using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class GiohangSanpham
{
    public string MaGioHang { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public int SoLuong { get; set; }

    public double TongTien { get; set; }

    public virtual Giohang MaGioHangNavigation { get; set; } = null!;

    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
