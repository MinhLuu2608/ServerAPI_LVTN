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
            string selectString = @"Select *, convert(varchar, NgayTao, 103) as NgayTaoConvert,
                convert(varchar, NgayHen, 103) as NgayHenConvert,
                convert(varchar, NgayThu, 29) as NgayThuConvert
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

        [HttpPost("received")]
        public JsonResult PostReceivedDonHang(DonHang dh)
        {
            string selectString = @"Select *, convert(varchar, NgayTao, 103) as NgayTaoConvert,
                convert(varchar, NgayHen, 103) as NgayHenConvert,
                convert(varchar, NgayThu, 29) as NgayThuConvert
                from DonHangDV 
                join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien
                join Account on Account.IDAccount = DonHangDV.IDAccount ";
            string whereString = " where NhanVien.IDNhanVien = " + dh.IDNhanVien + 
                " and TenKhachHang like N'%" + dh.TenKhachHang + "%' ";
            string orderByString = " order by NgayHen";
            if (dh.TinhTrangXuLy != "-1")
            {
                whereString = string.Concat(whereString, " and TinhTrangXuLy = N'" + dh.TinhTrangXuLy + "'");
            }
            string query = string.Concat(selectString, whereString, orderByString);
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

        [HttpPost("confirm")]
        public JsonResult PostConfirmCompleteDonHang(DonHang dh)
        {

            string checkQuery = @"Select * from DonHangDV where NgayThu is not null and IDDonhang = " + dh.IDDonHang;
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
                if (tblCheck.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Đơn hàng đã hoàn thành không thể thực hiện thao tác này. Hãy kiểm tra lại"
                    }
                    );
                }

                string queryUpdateDonHang = @"Update DonHangDV set NgayThu = SYSDATETIME(),
                    TinhTrangXuLy = N'Đã hoàn thành' where IDDonHang = " + dh.IDDonHang;
                Console.WriteLine(queryUpdateDonHang);
                using (SqlCommand myCommand = new SqlCommand(queryUpdateDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Xác nhận hoàn thành và thanh toán đơn hàng thành công."
                });
            }
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
                if(DateTime.Compare(ngayHen, today) < 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Ngày Hẹn không thể nhỏ hơn ngày hiện tại"
                    }
                    );
                }

                string queryUpdateDonHang = @"Update DonHangDV set NgayHen = '" + dh.NgayHen + @"', 
                    BuoiHen = N'" + dh.BuoiHen + @"', TinhTrangXuLy = N'Đã tiếp nhận' 
                    where IDDonHang = " + dh.IDDonhang;
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

        [HttpPost("cancel")]
        public JsonResult PostCancelDonHang(DonHang dh)
        {

            string checkQuery = @"Select * from DonHangDV where NgayHen is not null and IDDonhang = " + dh.IDDonHang;
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
                if (tblCheck.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Đơn hàng này đã được tiếp nhận. Hãy kiểm tra lại"
                    }
                    );
                }

                string queryUpdateDonHang = @"Update DonHangDV set Note = N'" + dh.Note + @"',
                    TinhTrangXuLy = N'Đã bị huỷ' where IDDonHang = " + dh.IDDonHang;
                string queryInsertTiepNhanDH = @"Insert into ChiTietTiepNhanDonHang values(
                    " + dh.IDDonHang + ", " + dh.IDNhanVien + ")";
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
                    message = "Huỷ đơn hàng thành công."
                });
            }
        }

        [HttpPost("cancelreceveid")]
        public JsonResult PostCancelReceivedDonHang(DonHang dh)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string checkQuery = @"Select * from DonHangDV where TinhTrangXuLy = N'Đã hoàn thành' and IDDonHang = " + dh.IDDonHang;
            DataTable tblCheck = new DataTable();

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
                        message = "Đơn hàng đã hoàn thành không thể thực hiện thao tác này."
                    });
                }

                string queryUpdateDonHang = @"Update DonHangDV set Note = N'" + dh.Note + @"',
                    TinhTrangXuLy = N'Đã bị huỷ' where IDDonHang = " + dh.IDDonHang;
                using (SqlCommand myCommand = new SqlCommand(queryUpdateDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Huỷ đơn hàng thành công."
                });
            }
        }

        [HttpPut("recovery")]
        public JsonResult PutRecoveryDonHang(DonHang dh)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                string queryUpdateDonHang = @"Update DonHangDV set Note = N'',
                    TinhTrangXuLy = N'Đã tiếp nhận' where IDDonHang = " + dh.IDDonHang;
                using (SqlCommand myCommand = new SqlCommand(queryUpdateDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Phục hồi đơn hàng thành công."
                });
            }
        }

        [HttpPut("editInfo")]
        public JsonResult PutEditInfoDonHang(DonHang dh)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                string queryUpdateTongTienDonHang = @"Update DonHangDV set TenKhachHang = " + dh.TenKhachHang + @",
                    DiaChiKH = " + dh.DiaChiKH + ", SoDienThoaiKH = " + dh.SoDienThoaiKH + @"
                    where IDDonHang =" + dh.IDDonHang;
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Chỉnh sửa thông tin đơn hàng thành công."
                });
            }
        }

        [HttpPut("editDichVu")]
        public JsonResult PutEditDichVuDonHang(ChiTietDonHang ct)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                string queryUpdateChiTietDonHang = @"Update ChiTietDonHang set DonGia = " + ct.DonGia + @",
                    SoLuong = " + ct.SoLuong + " TongTienDV = " + ct.TongTien +
                    "where IDDonHang = " + ct.IDDonHang + " IDDichVu = " + ct.IDDichVu;
                using (SqlCommand myCommand = new SqlCommand(queryUpdateChiTietDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                string queryGetTongTienDH = @"Select Sum(TongTienDV) from ChiTietDonHang where IDDonHang = " + ct.IDDonHang;
                DataTable tblTongTienDH = new DataTable();
                using (SqlCommand myCommand = new SqlCommand(queryGetTongTienDH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblTongTienDH.Load(myReader);
                    myReader.Close();
                }
                string tongTienDH = tblTongTienDH.Rows[0][0].ToString();
                string queryUpdateTongTienDonHang = @"Update DonHangDV set TongTienDH = " + tongTienDH + @"  
                    where IDDonHang =" + ct.IDDonHang;
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Chỉnh sửa dịch vụ đơn hàng thành công."
                });
            }
        }
    }
}
