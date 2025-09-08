using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Nhasanxuat
{
    public string MaNhaSx { get; set; } = null!;

    public string TenNhaSx { get; set; } = null!;

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
