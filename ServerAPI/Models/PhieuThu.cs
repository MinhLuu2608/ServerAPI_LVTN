namespace ServerAPI.Models
{
    public class PhieuThu
    {
        public int IDPhieu { get; set; }
        public int? IDKhachHang { get; set; }
        public int? IDTuyenThu { get; set; }
        public int? IDKyThu { get; set; }
        public int? IDNhanVien { get; set; }
        public string? NgayTao { get; set; }
        public string? NgayThu { get; set; }
    }
}
