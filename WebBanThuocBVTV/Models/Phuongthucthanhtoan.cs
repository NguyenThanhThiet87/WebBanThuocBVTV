using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Phuongthucthanhtoan
{
    public string MaPhuongThucTt { get; set; } = null!;

    public string? TenPhuongThucTt { get; set; }

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
