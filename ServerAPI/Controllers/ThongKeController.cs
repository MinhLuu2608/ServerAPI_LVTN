using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

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
            if(loai == -1)
            {
                query = @"
                    select ThangThu, Sum(DoanhThu) as DoanhThu
                    from (
	                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                            SUM(Gia) as DoanhThu
	                    from HoaDon 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                    group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar))
	                    union
	                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                            SUM(TongTienDH) as DoanhThu
	                    from DonHangDV 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                    group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar))
	                    ) as DoanhThu
                    group by ThangThu
                ";
            }
            if(loai == 1)
            {
                query = @"
                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                        SUM(Gia) as DoanhThu
	                from HoaDon 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar))
                ";
            }
            if(loai == 2)
            {
                query = @"
                    select Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar)) as ThangThu, 
                        SUM(TongTienDH) as DoanhThu
	                from DonHangDV 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as varchar) like '" + nam + @"'
	                group by Concat(N'Tháng ', Cast(Month(CAST(NgayThu as datetime)) as varchar))
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
                    select NamThu, Sum(DoanhThu) as DoanhThu
                    from (
	                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                            SUM(Gia) as DoanhThu
	                    from HoaDon 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                    group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar))
	                    union
	                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                            SUM(TongTienDH) as DoanhThu
	                    from DonHangDV 
                        where NgayThu is not null and 
                            Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                    group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar))
	                    ) as DoanhThu
                    group by NamThu
                ";
            }
            if (loai == 1)
            {
                query = @"
                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                        SUM(Gia) as DoanhThu
	                from HoaDon 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar))
                ";
            }
            if (loai == 2)
            {
                query = @"
                    select Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar)) as NamThu, 
                        SUM(TongTienDH) as DoanhThu
	                from DonHangDV 
                    where NgayThu is not null and 
                        Cast(YEAR(CAST(NgayThu as datetime)) as int) between " + tuNam + " and " + denNam + @"
	                group by Concat(N'Năm ', Cast(Year(CAST(NgayThu as datetime)) as varchar))
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
    }
}
