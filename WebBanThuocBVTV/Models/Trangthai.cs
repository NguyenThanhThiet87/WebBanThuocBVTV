using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Trangthai
{
    public string MaTrangThai { get; set; } = null!;

    public string? TenTrangThai { get; set; }

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
