using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileAppController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public MobileAppController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{username}/{password}")]
        public JsonResult CheckLogin(string username, string password)
        {
            string query = "Select * from account where username = '" + username + "' and password = '" + password + "'";
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
            if(table.Rows.Count > 0)
            {
                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }
            
        }

        [HttpGet("getIDAccount/{username}/{password}")]
        public JsonResult getIDAccount(string username, string password)
        {
            string query = "Select IDAccount from account where username = '" + username + "' and password = '" + password + "'";
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

            return new JsonResult(int.Parse(table.Rows[0][0].ToString()));

        }

        [HttpGet("{username}/{password}/{SDT}")]
        public JsonResult RegisterAccount(string username, string password, string sdt)
        {
            string queryCheckUsername = @"Select * from Account where username = '" + username + "'";
            DataTable tblCheckUsername = new DataTable();
            string queryCheckSDT = @"Select * from Account where SDT = '" + sdt + "'";
            DataTable tblCheckSDT = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(queryCheckUsername, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckUsername.Load(myReader);
                    myReader.Close();
                }
                if(tblCheckUsername.Rows.Count > 0)
                {

                    myCon.Close();
                    return new JsonResult("Username đã tồn tại");
                }

                using (SqlCommand myCommand = new SqlCommand(queryCheckSDT, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckSDT.Load(myReader);
                    myReader.Close();
                }
                if(tblCheckSDT.Rows.Count > 0)
                {

                    myCon.Close();
                    return new JsonResult("Số điện thoại đã tồn tại");
                }

                string queryGetIDAccount = "Select max(IDAccount) + 1 from Account";
                DataTable tblIDAccount = new DataTable();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblIDAccount.Load(myReader);
                    myReader.Close();
                }
                string IDAccount = tblIDAccount.Rows[0][0].ToString();
                string queryInsertAccount = @"Insert into Account values 
                    (" + IDAccount + ", '" + username + "', '" + password + "', '" + sdt +"', 'anonymous.png')";
                using (SqlCommand myCommand = new SqlCommand(queryInsertAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Đăng ký thành công");
            }
        }

        [HttpGet("getHoaDon/{IDAccount}")]
        public JsonResult getHDToKHByIDAccount(int IDAccount)
        {
            string queryGetHoaDon = @"Select HoaDon.IDHoaDon, HoaDon.MaSoPhieu, 
                format(HoaDon.NgayTao, 'dd/MM/yy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yy') as NgayThu, 
                KyThu.TenKyThu, KyThu.Thang, Account.IDAccount,
                KhachHang.MaKhachHang, KhachHang.HoTenKH, LoaiKhachHang.Gia, LoaiKhachHang.TenLoai,
                CONCAT(Diachi, N', Phường ' , TenXaPhuong, ', ' , TenQuanHuyen) as DiaChiKH
                from HoaDon
                join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu 
                join KhachHang on KhachHang.IDKhachHang = HoaDon.IDKhachHang 
				join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
				join QuanHuyen on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang 
                join LienKetTK on KhachHang.IDKhachHang = LienKetTK.IDKhachHang 
                join Account on Account.IDAccount = LienKetTK.IDAccount 
                where Account.IDAccount = " + IDAccount + " order by TenKyThu desc ";

            DataTable table = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetHoaDon, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);

        }

        [HttpPost("link")]
        public JsonResult PostLinkAccount(LinkedAccount linkAccount)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryGetIDKH = "Select IDKhachHang from KhachHang where MaKhachHang = '" + linkAccount.MaKH + "'";
            DataTable tblIDKH = new DataTable();
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDKH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblIDKH.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if(tblIDKH.Rows.Count == 0)
            {
                return new JsonResult("Mã khách hàng không tồn tại");
            }
            string IDKhachHang = tblIDKH.Rows[0][0].ToString();

            string queryCheckIsLinked = "Select * from LienKetTK where IDAccount = " + linkAccount.IDAccount + 
                " and IDKhachHang = '" + IDKhachHang + "'";
            DataTable tblCheckIsLinked = new DataTable();


            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheckIsLinked, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckIsLinked.Load(myReader);
                    myReader.Close();
                }
                if(tblCheckIsLinked.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult("Đã liên kết với mã khách hàng này");
                }

                string queryInsert = "Insert into LienKetTK values(" + linkAccount.IDAccount + ", " + IDKhachHang + ")";
                using (SqlCommand myCommand = new SqlCommand(queryInsert, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Liên kết thành công");
            }
        }
    }
}
