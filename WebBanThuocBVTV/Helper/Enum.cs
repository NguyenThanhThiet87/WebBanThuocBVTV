using System.ComponentModel.DataAnnotations;

namespace WebBanThuocBVTV.Helper
{
    public enum SideBar
    {
        TongQuan,
        SanPham, 
        DonHang,
        KhachHang,
        BinhLuan,
        NhanVien,
        HeThong
    }

    public enum SortOptions
    {
        [Display(Name = "Id thấp đến cao")]
        IdAsc,
        [Display(Name = "Id cao đến thấp")]
        IdDesc,
        [Display(Name = "Tên A-Z")]
        NameA_Z,
        [Display(Name = "Tên Z-A")]
        NameZ_A,
        [Display(Name = "Giá thấp đến cao")]
        PriceAsc,
        [Display(Name = "Giá cao đến thấp")]
        PriceDesc,
        [Display(Name = "Số lượng ít đến nhiều")]
        QuantityAsc,
        [Display(Name = "Số lượng nhiều đến ít")]
        QuantityDesc
    }

    public enum SortOptionsCustomer
    {
        [Display(Name = "Id thấp đến cao")]
        IdAsc,
        [Display(Name = "Id cao đến thấp")]
        IdDesc,
        [Display(Name = "Tên A-Z")]
        NameA_Z,
        [Display(Name = "Tên Z-A")]
        NameZ_A,
        [Display(Name = "Ngày tạo mới nhất")]
        DateAsc,
        [Display(Name = "Ngày tạo cũ nhất")]
        DateDesc,
        [Display(Name = "Tuổi tăng dần")]
        AgeAsc,
        [Display(Name = "Tuổi giảm dần")]
        AgeDesc
    }
    public enum GenderOptions{
        [Display(Name ="Tất cả")]
        All,
        [Display(Name = "Nữ")]
        Nu,
        [Display(Name = "Nam")]
        Nam
    }
    public enum CreateAtOptions
    {
        [Display(Name = "Tất cả")]
        All,
        [Display(Name = "7 ngày trước")]
        Week,
        [Display(Name = "1 tháng trước")]
        Month,
        [Display(Name = "1 năm trước")]
        Year
    }
    public enum PriceArrange
    {
        [Display(Name ="Dưới 150,000đ")]
        Bel150,
        [Display(Name = "150,000đ - 350,000đ")]
        fr150t350,
        [Display(Name = "Trên 350,000đ")]
        Abo350,
    }
    public enum QuantityOptions
    {
        [Display(Name = "Còn hàng")]
        Avaiable,
        [Display(Name = "Hết hàng (< 10)")]
        OutOfShock
    }
    public enum SortOptionsOrder
    {
        [Display(Name = "Ngày tạo mới nhất")]
        DateAsc,
        [Display(Name = "Ngày tạo cũ nhất")]
        DateDesc,
        [Display(Name = "Giá thấp đến cao")]
        PriceAsc,
        [Display(Name = "Giá cao đến thấp")]
        PriceDesc
    }
    public enum IsActiveProduct
    {
        [Display(Name = "Đang Kinh Doanh")]
        Active,
        [Display(Name = "Ngừng Kinh Doanh")]
        None
    }
    public enum CategoryCustomer
    {
        [Display(Name = "Khách hàng")]
        KH,
        [Display(Name = "Vãng lai")]
        GU
    }
    public enum EvaluateOptions
    {
        [Display(Name = "1 sao")]
        one,
        [Display(Name = "2 sao")]
        two,
        [Display(Name = "3 sao")]
        three,
        [Display(Name = "4 sao")]
        four,
        [Display(Name = "5 sao")]
        five
    }
    public enum SortPrice
    {
        [Display(Name = "Giảm dần")]
        priceDesc,
        [Display(Name = "Tăng dần")]
        priceAsc
    }
    public enum StateComment
    {
        [Display(Name = "Mới nhất")]
        newComment,
        [Display(Name = "Cũ nhất")]
        oldComment
    }
    public enum IsReply
    {
        [Display(Name = "Chưa phản hồi")]
        none,
        [Display(Name = "Đã phản hồi")]
        done
    }
    
}
