using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Giaodich
{
    public string MaGiaoDich { get; set; } = null!;

    public string? MaDonHang { get; set; }

    public string? NoiDung { get; set; }

    public string? MaNganHang { get; set; }

    public double? TongTien { get; set; }

    public DateTime? ThoiGian { get; set; }

    public virtual Donhang? MaDonHangNavigation { get; set; }
}
