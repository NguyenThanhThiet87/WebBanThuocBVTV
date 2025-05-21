using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Danhgium
{
    public int MaDanhGia { get; set; }

    public virtual ICollection<Binhluan> Binhluans { get; set; } = new List<Binhluan>();
}
