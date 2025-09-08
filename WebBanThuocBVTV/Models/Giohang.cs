using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Giohang
{
    public string MaGioHang { get; set; } = null!;

    public double TongTien { get; set; }

    public string? MaNd { get; set; }

    public virtual ICollection<GiohangSanpham> GiohangSanphams { get; set; } = new List<GiohangSanpham>();

    public virtual Nguoidung? MaNdNavigation { get; set; }
}
