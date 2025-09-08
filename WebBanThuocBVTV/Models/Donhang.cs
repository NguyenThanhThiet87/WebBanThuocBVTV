using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Donhang
{
    public string MaDonHang { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public double TongTien { get; set; }

    public string MaNd { get; set; } = null!;

    public string MaTrangThai { get; set; } = null!;

    public string MaPhuongThucTt { get; set; } = null!;

    public string? GhiChu { get; set; }

    public DateTime? NgayGiaoHang { get; set; }

    public virtual ICollection<DonhangSanpham> DonhangSanphams { get; set; } = new List<DonhangSanpham>();

    public virtual ICollection<Giaodich> Giaodiches { get; set; } = new List<Giaodich>();

    public virtual Nguoidung MaNdNavigation { get; set; } = null!;

    public virtual Phuongthucthanhtoan MaPhuongThucTtNavigation { get; set; } = null!;

    public virtual Trangthai MaTrangThaiNavigation { get; set; } = null!;
}
