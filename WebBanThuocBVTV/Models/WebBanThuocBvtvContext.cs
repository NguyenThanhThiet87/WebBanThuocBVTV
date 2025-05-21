using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebBanThuocBVTV.Models;

public partial class WebBanThuocBvtvContext : DbContext
{
    public WebBanThuocBvtvContext()
    {
    }

    public WebBanThuocBvtvContext(DbContextOptions<WebBanThuocBvtvContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Binhluan> Binhluans { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<DonhangSanpham> DonhangSanphams { get; set; }

    public virtual DbSet<Giohang> Giohangs { get; set; }

    public virtual DbSet<GiohangSanpham> GiohangSanphams { get; set; }

    public virtual DbSet<Hinhanh> Hinhanhs { get; set; }

    public virtual DbSet<Nguoidung> Nguoidungs { get; set; }

    public virtual DbSet<Nhasanxuat> Nhasanxuats { get; set; }

    public virtual DbSet<Nhomsanpham> Nhomsanphams { get; set; }

    public virtual DbSet<Phuongthucthanhtoan> Phuongthucthanhtoans { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Trangthai> Trangthais { get; set; }

    public virtual DbSet<Vaitro> Vaitros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=THANHTHIET;Initial Catalog=WEB_BAN_THUOC_BVTV;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Binhluan>(entity =>
        {
            entity.HasKey(e => new { e.MaNd, e.MaSanPham, e.ThoiGian });

            entity.ToTable("BINHLUAN");

            entity.Property(e => e.MaNd)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaND");
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ThoiGian).HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(1024);

            entity.HasOne(d => d.MaDanhGiaNavigation).WithMany(p => p.Binhluans)
                .HasForeignKey(d => d.MaDanhGia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BINHLUAN_DANHGIA");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Binhluans)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BINHLUAN_NGUOIDUNG");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.Binhluans)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BINHLUAN_SANPHAM");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.MaDanhGia).HasName("PK__DANHGIA__AA9515BF211F6315");

            entity.ToTable("DANHGIA");

            entity.Property(e => e.MaDanhGia).ValueGeneratedNever();
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DONHANG__129584AD9582A6C9");

            entity.ToTable("DONHANG");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNd)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaND");
            entity.Property(e => e.MaPhuongThucTt)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaPhuongThucTT");
            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayLap).HasColumnType("datetime");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaNd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOIDUNG_DONHANG");

            entity.HasOne(d => d.MaPhuongThucTtNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaPhuongThucTt)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THANHTOAN_DONHANG");

            entity.HasOne(d => d.MaTrangThaiNavigation).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaTrangThai)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TRANGTHAI_DONHANG");
        });

        modelBuilder.Entity<DonhangSanpham>(entity =>
        {
            entity.HasKey(e => new { e.MaDonHang, e.MaSanPham });

            entity.ToTable("DONHANG_SANPHAM");

            entity.Property(e => e.MaDonHang)
                .HasMaxLength(36)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.DonhangSanphams)
                .HasForeignKey(d => d.MaDonHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PK_DONHANG_SANPHAM_DONHANG");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.DonhangSanphams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PK_DONHANG_SANPHAM_SANPHAM");
        });

        modelBuilder.Entity<Giohang>(entity =>
        {
            entity.HasKey(e => e.MaGioHang).HasName("PK__GIOHANG__F5001DA34201ECA6");

            entity.ToTable("GIOHANG");

            entity.Property(e => e.MaGioHang)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaNd)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaND");

            entity.HasOne(d => d.MaNdNavigation).WithMany(p => p.Giohangs)
                .HasForeignKey(d => d.MaNd)
                .HasConstraintName("FK_GIOHANG_NGUOIDUNG");
        });

        modelBuilder.Entity<GiohangSanpham>(entity =>
        {
            entity.HasKey(e => new { e.MaGioHang, e.MaSanPham });

            entity.ToTable("GIOHANG_SANPHAM");

            entity.Property(e => e.MaGioHang)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaGioHangNavigation).WithMany(p => p.GiohangSanphams)
                .HasForeignKey(d => d.MaGioHang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GIOHANG_SANPHAM_GIOHANG");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.GiohangSanphams)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GIOHANG_SANPHAM_SANPHAM");
        });

        modelBuilder.Entity<Hinhanh>(entity =>
        {
            entity.HasKey(e => e.MaHinhAnh).HasName("PK__HINHANH__A9C37A9B615D3AF5");

            entity.ToTable("HINHANH");

            entity.Property(e => e.MaHinhAnh)
                .HasMaxLength(7)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.MaSanPham)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Url).HasMaxLength(255);

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.Hinhanhs)
                .HasForeignKey(d => d.MaSanPham)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HINHANH_SANPHAM");
        });

        modelBuilder.Entity<Nguoidung>(entity =>
        {
            entity.HasKey(e => e.MaNd).HasName("PK__NGUOIDUN__2725D7247BB6C94A");

            entity.ToTable("NGUOIDUNG");

            entity.Property(e => e.MaNd)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaND");
            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.HoTen).HasMaxLength(30);
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.NgayBdlv).HasColumnName("NgayBDLV");
            entity.Property(e => e.PassWord)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.Nguoidungs)
                .HasForeignKey(d => d.MaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NGUOIDUNG_VAITRO");
        });

        modelBuilder.Entity<Nhasanxuat>(entity =>
        {
            entity.HasKey(e => e.MaNhaSx).HasName("PK__NHASANXU__C87A6D20449EF457");

            entity.ToTable("NHASANXUAT");

            entity.Property(e => e.MaNhaSx)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNhaSX");
            entity.Property(e => e.TenNhaSx)
                .HasMaxLength(30)
                .HasColumnName("TenNhaSX");
        });

        modelBuilder.Entity<Nhomsanpham>(entity =>
        {
            entity.HasKey(e => e.MaNhomSp).HasName("PK__NHOMSANP__5A1AD2F952051279");

            entity.ToTable("NHOMSANPHAM");

            entity.Property(e => e.MaNhomSp)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNhomSP");
            entity.Property(e => e.TenNhomSp)
                .HasMaxLength(30)
                .HasColumnName("TenNhomSP");
        });

        modelBuilder.Entity<Phuongthucthanhtoan>(entity =>
        {
            entity.HasKey(e => e.MaPhuongThucTt).HasName("PK__PHUONGTH__2AC557DF114076BF");

            entity.ToTable("PHUONGTHUCTHANHTOAN");

            entity.Property(e => e.MaPhuongThucTt)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaPhuongThucTT");
            entity.Property(e => e.TenPhuongThucTt)
                .HasMaxLength(30)
                .HasColumnName("TenPhuongThucTT");
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SANPHAM__FAC7442D4658BE1F");

            entity.ToTable("SANPHAM");

            entity.Property(e => e.MaSanPham)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.CongDung).HasMaxLength(2500);
            entity.Property(e => e.HuongDanSd)
                .HasMaxLength(2500)
                .HasColumnName("HuongDanSD");
            entity.Property(e => e.MaNhaSx)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNhaSX");
            entity.Property(e => e.MaNhomSp)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("MaNhomSP");
            entity.Property(e => e.TenSanPham).HasMaxLength(30);
            entity.Property(e => e.ThanhPhan).HasMaxLength(500);

            entity.HasOne(d => d.MaNhaSxNavigation).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.MaNhaSx)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_NHASX");

            entity.HasOne(d => d.MaNhomSpNavigation).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.MaNhomSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_NHOM");
        });

        modelBuilder.Entity<Trangthai>(entity =>
        {
            entity.HasKey(e => e.MaTrangThai).HasName("PK__TRANGTHA__AADE4138D2C56548");

            entity.ToTable("TRANGTHAI");

            entity.Property(e => e.MaTrangThai)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenTrangThai).HasMaxLength(30);
        });

        modelBuilder.Entity<Vaitro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VAITRO__C24C41CF6527D19E");

            entity.ToTable("VAITRO");

            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TenVaiTro).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
