using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DonHangController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet()]
        public JsonResult GetDonHang()
        {
            string query = @"Select *, convert(varchar, NgayTao, 103) as NgayTaoConvert
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien
                join Account on Account.IDAccount = DonHangDV.IDAccount";
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

        [HttpGet("chitiet/{idDH}")]
        public JsonResult GetChiTietDonHang(int idDH)
        {
            string query = @"Select *, convert(varchar, NgayTao, 103) as NgayTaoConvert
                from DonHangDV 
                join ChiTietDonHang on DonHangDV.IDDonHang = ChiTietDonHang.IDDonHang
                join DichVu on DichVu.IDDichVu = ChiTietDonHang.IDDichVu
                where DonHangDV.IDDonHang = " + idDH;
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

        [HttpPost()]
        public JsonResult PostDonHang(DonHang dh)
        {
            string selectString = @"Select *, convert(varchar, NgayTao, 103) as NgayTaoConvert
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien
                join Account on Account.IDAccount = DonHangDV.IDAccount ";
            string whereString = " where TenKhachHang like N'%" + dh.TenKhachHang + "%' ";
            if(dh.TinhTrangXuLy != "-1")
            {
                whereString = string.Concat(whereString, " and TinhTrangXuLy = N'" + dh.TinhTrangXuLy + "'");
            }
            string query = string.Concat(selectString, whereString);
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

        [HttpPost("accept")]
        public JsonResult PostAcceptDonHang(TiepNhanDH dh)
        {

            string checkQuery = @"Select * from DonHangDV where NgayHen is not null and IDDonhang = " + dh.IDDonhang;
            DataTable tblCheck = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheck.Load(myReader);
                    myReader.Close();
                }
                if(tblCheck.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Đơn hàng này đã được tiếp nhận. Hãy kiểm tra lại"
                    }
                    );
                }

                DateTime ngayHen = DateTime.Parse(dh.NgayHen);
                DateTime today = DateTime.Today;
                if(DateTime.Compare(ngayHen, today) <= 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Ngày Hẹn phải lớn hơn ngày hiện tại"
                    }
                    );
                }

                string queryUpdateDonHang = @"Update DonHangDV set NgayHen = '" + dh.NgayHen + @"', 
                    BuoiHen = N'" + dh.BuoiHen + @"', TinhTrangXuLy = N'Đã tiếp nhận' 
                    where IDDonHang = " + dh.IDDonhang;
                Console.WriteLine(queryUpdateDonHang);
                string queryInsertTiepNhanDH = @"Insert into ChiTietTiepNhanDonHang values(
                    " + dh.IDDonhang + ", " + dh.IDNhanVien + ")";
                using (SqlCommand myCommand = new SqlCommand(queryUpdateDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(queryInsertTiepNhanDH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Tiếp nhận đơn hàng thành công."
                });
            }
        }
    }
}
