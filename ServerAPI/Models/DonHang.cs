namespace ServerAPI.Models
{
    public class DonHang
    {
        public int? IDDonhang { get; set; }
        public int? IDNhanVien { get; set; }
        public string? MaDonHang { get; set; }
        public string? TenKhachHang { get; set; }
        public DateTime? NgayHen { get; set; }
        public string? BuoiHen { get; set; }
        public string? TinhTrangXuLy { get; set; }
    }
}
