using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Nhomsanpham
{
    public string MaNhomSp { get; set; } = null!;

    public string TenNhomSp { get; set; } = null!;

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
