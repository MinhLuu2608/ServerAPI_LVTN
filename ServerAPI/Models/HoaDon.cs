namespace ServerAPI.Models
{
    public class HoaDon
    {
        public int IDHoaDon { get; set; }
        public int? IDKhachHang { get; set; }
        public int? IDTuyenThu { get; set; }
        public int? IDKyThu { get; set; }
        public int? IDNhanVien { get; set; }
        public string? NgayTao { get; set; }
        public string? NgayThu { get; set; }
        public int? Gia { get; set; }
    }
}
