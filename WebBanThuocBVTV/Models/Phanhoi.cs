using System;
using System.Collections.Generic;

namespace WebBanThuocBVTV.Models;

public partial class Phanhoi
{
    public int MaPhanHoi { get; set; }

    public DateTime ThoiGianBinhLuan { get; set; }

    public string MaNdBinhLuan { get; set; } = null!;

    public string MaSpBinhLuan { get; set; } = null!;

    public string MaNhanVien { get; set; } = null!;

    public string NoiDungPhanHoi { get; set; } = null!;

    public DateTime NgayPhanHoi { get; set; }

    public virtual Binhluan Binhluan { get; set; } = null!;

    public virtual Nguoidung MaNhanVienNavigation { get; set; } = null!;
}
