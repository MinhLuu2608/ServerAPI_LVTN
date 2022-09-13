namespace ServerAPI.Models
{
    public class DonHang
    {
        public int? IDDonHang { get; set; }
        public int? IDAccount { get; set; }
        public int? IDNhanVien { get; set; }
        public string? MaDonHang { get; set; }
        public string? TenKhachHang { get; set; }
        public string? DiaChiKH { get; set; }
        public string? SoDienThoaiKH { get; set; }
        public DateTime? NgayHen { get; set; }
        public string? BuoiHen { get; set; }
        public string? Note { get; set; }
        public string? TinhTrangXuLy { get; set; }
        public int? TongTienDH { get; set; }

        public Service[]? DichVuList { get; set; }
    }
}
