using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class DonhangSanpham
{
    public string MaDonHang { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public int SoLuongDatMua { get; set; }

    public double TongTien { get; set; }

    public virtual Donhang MaDonHangNavigation { get; set; } = null!;

    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
