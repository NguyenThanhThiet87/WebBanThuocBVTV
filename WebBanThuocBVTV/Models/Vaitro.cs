using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Vaitro
{
    public string MaVaiTro { get; set; } = null!;

    public string? TenVaiTro { get; set; }

    public virtual ICollection<Nguoidung> Nguoidungs { get; set; } = new List<Nguoidung>();
}
