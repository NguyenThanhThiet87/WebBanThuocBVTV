using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Hinhanh
{
    public string MaHinhAnh { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string MaSanPham { get; set; } = null!;

    public virtual Sanpham MaSanPhamNavigation { get; set; } = null!;
}
