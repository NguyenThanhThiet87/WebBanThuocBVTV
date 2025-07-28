using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Sanpham
{
    public string MaSanPham { get; set; } = null!;

    public string TenSanPham { get; set; } = null!;

    public string? ThanhPhan { get; set; }

    public string? CongDung { get; set; }

    public string? HuongDanSd { get; set; }

    public double Gia { get; set; } = 0!;

    public int SoLuong { get; set; }

    public string MaNhomSp { get; set; } = null!;

    public string MaNhaSx { get; set; } = null!;

    public DateOnly? HanSd { get; set; }

    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();

    public virtual ICollection<DonhangSanpham> DonhangSanphams { get; set; } = new List<DonhangSanpham>();

    public virtual ICollection<GiohangSanpham> GiohangSanphams { get; set; } = new List<GiohangSanpham>();

    public virtual ICollection<Hinhanh> Hinhanhs { get; set; } = new List<Hinhanh>();

    public virtual Nhasanxuat MaNhaSxNavigation { get; set; } = null!;

    public virtual Nhomsanpham MaNhomSpNavigation { get; set; } = null!;
}
