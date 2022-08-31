using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

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
    }
}
