namespace ServerAPI.Models
{
    public class KhachHang
    {

        public int? IDKhachHang { get; set; }
        public int IDXaPhuong { get; set; }
        public int IDLoaiKhachHang { get; set; }
        public string? MaKhachHang { get; set; }
        public string HoTenKH { get; set; }
        public string DiaChi { get; set; }
        public string CCCD { get; set; }    
        public DateTime NgayCap { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? NgayChinhSua { get; set; }
        public int? TrangThai { get; set; }
    }
}
