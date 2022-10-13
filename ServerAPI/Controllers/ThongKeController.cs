using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThongKeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public ThongKeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("doanhthutheothang/{loai}/{nam}")]
        public JsonResult GetDoanhThuTheoThang(int loai, int nam)
        {
            // loai == -1 : Tất cả
            // loai == 1: Hóa đơn tháng
            // loai == 2: Đơn hàng dịch vụ
            string query = "";
            if (loai == -1)
            {
                query = @"
                    select ThangThu, Sum(DoanhThu) as 'Doanh thu'
                    from (
	                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                            Cast(Month(CAST(NgayThu as datetime)) as int) as Number, SUM(Gia) as DoanhThu
	                    from HoaDon 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                    group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)), 
                            Cast(Month(CAST(NgayThu as datetime)) as int)
	                    union
	                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                            Cast(Month(CAST(NgayThu as datetime)) as int) as Number, SUM(TongTienDH) as DoanhThu
	                    from DonHangDV 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                    group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)), 
                            Cast(Month(CAST(NgayThu as datetime)) as int)
	                    ) as DoanhThu
                    group by ThangThu, Number
					order by Number
                ";
            }
            if (loai == 1)
            {
                query = @"
                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                        Cast(Month(CAST(NgayThu as datetime)) as int) as Number, SUM(Gia) as 'Doanh thu'
	                from HoaDon 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)), 
                        Cast(Month(CAST(NgayThu as datetime)) as int)
                    order by Number
                ";
            }
            if (loai == 2)
            {
                query = @"
                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                        Cast(Month(CAST(NgayThu as datetime)) as int) as Number, SUM(TongTienDH) as 'Doanh thu'
	                from DonHangDV 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)), 
                        Cast(Month(CAST(NgayThu as datetime)) as int)
                    order by Number
                ";
            }
            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("doanhthutheonam/{loai}/{tuNam}/{denNam}")]
        public JsonResult GetDoanhThuTheoNam(int loai, int tuNam, int denNam)
        {
            // loai == -1 : Tất cả
            // loai == 1: Hóa đơn tháng
            // loai == 2: Đơn hàng dịch vụ
            string query = "";
            if (loai == -1)
            {
                query = @"
                    select NamThu, Sum(DoanhThu) as 'Doanh thu'
                    from (
	                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                            Cast(Year(CAST(NgayThu as datetime)) as int) as Number, SUM(Gia) as DoanhThu
	                    from HoaDon 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                    group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)),
							Cast(Year(CAST(NgayThu as datetime)) as int)
	                    union
	                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                            Cast(Year(CAST(NgayThu as datetime)) as int) as Number, SUM(TongTienDH) as DoanhThu
	                    from DonHangDV 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                    group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)),
							Cast(Year(CAST(NgayThu as datetime)) as int)
	                    ) as DoanhThu
                    group by NamThu, Number
					order by Number
                ";
            }
            if (loai == 1)
            {
                query = @"
                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                        Cast(Year(CAST(NgayThu as datetime)) as int) as Number, SUM(Gia) as 'Doanh thu'
	                from HoaDon 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)),
						Cast(Year(CAST(NgayThu as datetime)) as int)
                    order by Number
                ";
            }
            if (loai == 2)
            {
                query = @"
                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                        Cast(Year(CAST(NgayThu as datetime)) as int) as Number, SUM(TongTienDH) as 'Doanh thu'
	                from DonHangDV 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)),
					    Cast(Year(CAST(NgayThu as datetime)) as int)
                    order by Number
                ";
            }
            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("khachhangtheoquan/")]
        public JsonResult GetKhachHangTheoQuan()
        {
            string query = @"select TenQuanHuyen, Count(*) as N'Khách hàng' 
                from QuanHuyen
                join XaPhuong on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                where NgayKetThuc is null
                group by TenQuanHuyen
            ";
            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("khachhangtheotuyen/{idQuanHuyen}")]
        public JsonResult GetKhachHangTheoTuyen(int idQuanHuyen)
        {
            string query = @"Select TenTuyenThu, count(*) as N'Khách hàng'
                from TuyenThu
                join PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                join QuanHuyen on QuanHuyen.IDQuanHuyen = PhanTuyen.IDQuanHuyen
                join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                where KhachHang.NgayKetThuc is null and PhanTuyen.NgayKetThuc is null 
                    and QuanHuyen.IDQuanHuyen = " + idQuanHuyen + @"
                group by TenTuyenThu
            ";
            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet("khachhangtheoxp/{idTuyen}")]
        public JsonResult GetKhachHangTheoXP(int idTuyen)
        {
            string query = @"Select TenXaPhuong, count(*) as N'Khách hàng'
                from TuyenThu
                join PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                join QuanHuyen on QuanHuyen.IDQuanHuyen = PhanTuyen.IDQuanHuyen
                join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                where KhachHang.NgayKetThuc is null and PhanTuyen.NgayKetThuc is null 
                    and TuyenThu.IDTuyenThu = " + idTuyen + @"
                group by TenXaPhuong
            ";
            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }
    }


}
