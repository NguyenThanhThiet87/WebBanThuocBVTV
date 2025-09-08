using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Nguoidung
{
    public string MaNd { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public bool GioiTinh { get; set; }
    public string DisplayGioiTinh => GioiTinh ? "Nam" : "Nữ";

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public string? PassWord { get; set; }

    public DateTime? NgayBdlv { get; set; }

    public string MaVaiTro { get; set; } = null!;

    public string? GoogleId { get; set; }

    public DateTime? NgayTao { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? Avatar { get; set; }

    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();

    public virtual ICollection<Giohang> Giohangs { get; set; } = new List<Giohang>();

    public virtual Vaitro MaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<Phanhoi> Phanhois { get; set; } = new List<Phanhoi>();
}
