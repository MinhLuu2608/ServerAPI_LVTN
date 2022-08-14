using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhieuThuController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PhieuThuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //get
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select HoaDon.MaSoPhieu, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi,TuyenThu.MaTuyenThu,
                TuyenThu.IDTuyenThu,TuyenThu.TenTuyenThu,KyThu.IDKyThu,KyThu.TenKyThu,
	            KyThu.Thang,KyThu.Nam,NhanVien.MaNhanVien,NhanVien.IDNhanVien,NhanVien.HoTen,HoaDon.NgayTao,
                HoaDon.NgayThu, XaPhuong.IDXaPhuong,XaPhuong.TenXaPhuong,QuanHuyen.IDQuanHuyen,
                QuanHuyen.TenQuanHuyen,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia
                from HoaDon
                inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
				full outer join HDThanhToanTrucTiep on HDThanhToanTrucTiep.IDHoaDon = HoaDon.IDHoaDon
                FULL OUTER join TuyenThu on HDThanhToanTrucTiep.IDTuyenThu = TuyenThu.IDTuyenThu
                inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                FULL OUTER join NhanVien on HDThanhToanTrucTiep.IDNhanVien = NhanVien.IDNhanVien
                inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                   order by KhachHang.IDKhachHang,KyThu.Thang ASC
            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //get Phieu theo NV
        [HttpGet("nhanvien/{idNV}")]
        public JsonResult GetPhieuNV(int idNV)
        {
            string query = @"
               select PhieuThu.MaSoPhieu,PhieuThu.IDPhieu,KhachHang.MaKhachHang,KhachHang.IDKhachHang,KhachHang.HoTenKH,KhachHang.DiaChi,TuyenThu.MaTuyenThu,TuyenThu.IDTuyenThu,TuyenThu.TenTuyenThu,KyThu.IDKyThu,KyThu.TenKyThu,
					KyThu.Thang,KyThu.Nam,NhanVien.MaNhanVien,NhanVien.IDNhanVien,NhanVien.HoTen,PhieuThu.MauSoPhieu,PhieuThu.NgayTao,PhieuThu.NgayThu,XaPhuong.IDXaPhuong,XaPhuong.TenXaPhuong,QuanHuyen.IDQuanHuyen,QuanHuyen.TenQuanHuyen,LoaiKhachHang.IDLoaiKhachHang,LoaiKhachHang.TenLoai,LoaiKhachHang.Gia
                from PhieuThu
                inner join KhachHang
                on PhieuThu.IDKhachHang = KhachHang.IDKhachHang
                inner join TuyenThu
                on PhieuThu.IDTuyenThu = TuyenThu.IDTuyenThu
                inner join KyThu
                on PhieuThu.IDKyThu = KyThu.IDKyThu
                FULL OUTER join NhanVien
                on PhieuThu.IDNhanVien = NhanVien.IDNhanVien
                inner join XaPhuong
                on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join QuanHuyen
                on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                inner join LoaiKhachHang
                on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                join PhanTuyen 
                on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                    where PhanTuyen.IDNhanVien = " + idNV +
                  " order by KhachHang.IDKhachHang,KyThu.Thang ASC";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //get XaPhuong theo Tuyen Thu
        [HttpGet("getbyidemp/{idNhanVien}")]
        public JsonResult GetByStatus(int idNhanVien)
        {
            string query = @"
                select * from XaPhuong
                inner join PhanTuyen on XaPhuong.IDTuyenThu = PhanTuyen.IDTuyenThu
                where IDNhanVien = " + idNhanVien;
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
            myReader.Close();

            return new JsonResult(table);
        }
        //getKH
        [HttpGet("hotenKH")]
        public JsonResult GetHoTenKH()
        {
            string query = @"select * from KhachHang";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //getDiaChi(Quan Huyen)
        [HttpGet("quanhuyen")]
        public JsonResult GetQuanHuyen()
        {
            string query = @"select * from QuanHuyen;";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //getDiaChi (XaPhuong)
        [HttpGet("xaphuong")]
        public JsonResult GetXaPhuong()
        {
            string query = @"select * from XaPhuong;";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //get TuyenThu
        [HttpGet("tuyenthu")]
        public JsonResult GetTuyenThu()
        {
            string query = @"select * from TuyenThu;";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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

        //get TuyenThu
        [HttpGet("tuyenthu/{idNV}")]
        public JsonResult GetTuyenThuBYID(int idNV)
        {
            string query = @"select  TuyenThu.TenTuyenThu from TuyenThu 
                             inner join PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
                                where PhanTuyen.IDNhanVien = "+ idNV +" ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //get LoaiKhachHang
        [HttpGet("loaikhachhang")]
        public JsonResult GetLoaiKhachHang()
        {
            string query = @"select * from LoaiKhachHang;";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //getbyID
        [HttpGet("{id}")]
        public JsonResult GetByID(int id)
        {
            string query = @"select PhieuThu.IDPhieu,KhachHang.HoTenKH,TuyenThu.TenTuyenThu,KyThu.TenKyThu,
					            KyThu.Thang,KyThu.Nam,NhanVien.HoTen,PhieuThu.MauSoPhieu,
                                PhieuThu.NgayTao,PhieuThu.NgayThu from dbo.PhieuThu
                                 inner join KhachHang
                                    on PhieuThu.IDKhachHang = KhachHang.IDKhachHang
                                    inner join TuyenThu
                                    on PhieuThu.IDTuyenThu = TuyenThu.IDTuyenThu
                                    inner join KyThu
                                    on PhieuThu.IDKyThu = KyThu.IDKyThu
                                    inner join NhanVien
                                    on PhieuThu.IDNhanVien = NhanVien.IDNhanVien
                                where IDPhieu =" + id;
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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

        //get Ky Thu
        [HttpGet("kythu")]
        public JsonResult GetKyThu()
        {
            string query = @"select distinct KyThu.IDKyThu,KyThu.TenKyThu from KyThu";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //get KH
        [HttpGet("khachhang")]
        public JsonResult GetKhachHang()
        {
            string query = @"
               select distinct KhachHang.IDKhachHang,KhachHang.HoTenKH from KhachHang where TrangThai = 1                
            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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


        [HttpGet("khachhang/nhanvien/{idNV}")]
        public JsonResult GetKhachHangByNhanVien(int idNV)
        {
            string query = @"select distinct KhachHang.IDKhachHang,KhachHang.HoTenKH from KhachHang
                join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                where TrangThai = 1 and IDTuyenThu in 
                    (select IDTuyenThu from PhanTuyen where IDNhanVien = "+ idNV +@")";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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

        //get KH id
        [HttpGet("khachhang/{id}")]
        public JsonResult GetInfoKHByID(int id)
        {
            string query = @"select Diachi, IDTuyenThu 
                from dbo.KhachHang inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                where IDKhachHang =" + id;
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
        //insert
        [HttpPost]
        public JsonResult Post(PhieuThu pt)
        {
            string checkPhieuThu = @"Select * from PhieuThu where IDKhachHang = " + pt.IDKhachHang +
                @" and IDKyThu = " + pt.IDKyThu;
            DataTable tableCheckPhieuThu = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkPhieuThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tableCheckPhieuThu.Load(myReader);
                    myReader.Close();
                }
                if(tableCheckPhieuThu.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult("Phiếu thu tương ứng đã tồn tại. Không thể tạo thêm");
                }
                else
                {
                    string getMaxIDPhieuQuery = "select IDENT_CURRENT('PhieuThu') + 1";
                    string getIDLoaiKhachHang = "select IDLoaiKhachHang from KhachHang " +
                        "where IDKhachHang = " + pt.IDKhachHang;
                    string maxIDPhieu = "";
                    int IDLoaiKH = 0;
                    DataTable dt = new DataTable();
                    DataTable dtIDLoai = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(getMaxIDPhieuQuery, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dt.Load(myReader);
                        myReader.Close();
                    }
                    using (SqlCommand myCommand = new SqlCommand(getIDLoaiKhachHang, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dtIDLoai.Load(myReader);
                        myReader.Close();
                    }

                    maxIDPhieu = dt.Rows[0][0].ToString();
                    IDLoaiKH = int.Parse(dtIDLoai.Rows[0][0].ToString());
                    string maSoPhieu = "PT" + maxIDPhieu + "MKH" + pt.IDKhachHang + "D" + DateTime.Today.ToString("ddMMyyyy");
                    string query = @"insert into PhieuThu values (" + pt.IDKhachHang + @",
                        " + pt.IDTuyenThu + @"," + pt.IDKyThu + @",null,
                        '" + maSoPhieu + @"','" + IDLoaiKH + @"',GETDATE(),null)";
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                        myCon.Close();
                    }
                    return new JsonResult("Thêm phiếu thu thành công");
                }
            }
        }
        //delete
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                DELETE FROM PhieuThu 
                WHERE IDPhieu = '" + id + @"'
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
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
            return new JsonResult("Deleted Successfully");
        }
        //Put
        [HttpPut]
        public JsonResult Put(PhieuThu pt)
        {
            string getNgayThangKyThuQuery = @"select Thang, Nam from PhieuThu 
	            join KyThu on PhieuThu.IDKyThu = KyThu.IDKyThu
	            where IDPhieu = " + pt.IDPhieu;
            DataTable tableNgayThangKyThu = new DataTable();
            string query = @"
                UPDATE dbo.PhieuThu SET 
                      IDNhanVien = '" + pt.IDNhanVien + @"',
                      NgayThu = '" + DateTime.Now.ToString("yyyy-MM-dd") + @"'
                WHERE IDPhieu = '" + pt.IDPhieu + @"'
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getNgayThangKyThuQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tableNgayThangKyThu.Load(myReader);

                    myReader.Close();
                    myCon.Close();
                }
            }
            int thangNay = int.Parse(tableNgayThangKyThu.Rows[0][0].ToString());
            int namNay = int.Parse(tableNgayThangKyThu.Rows[0][1].ToString());
            int thangTruoc = 0;
            int namTruoc = 0;

            if (thangNay > 1 && thangNay < 13)
            {
                thangTruoc = thangNay - 1;
                namTruoc = namNay;
            }
            if (thangNay == 1)
            {
                thangTruoc = 12;
                namTruoc = namNay - 1;
            }

            string idKyThuTruoc = "";
            string getIDKyThuTruocQuery = @"Select KyThu.IDKyThu from KyThu where Thang = " + thangTruoc
                + @" and Nam = " + namTruoc;
            DataTable tblIDKyThuTruoc = new DataTable();
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getIDKyThuTruocQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblIDKyThuTruoc.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if (tblIDKyThuTruoc.Rows.Count > 0)
            {
                Console.WriteLine(tblIDKyThuTruoc.Rows[0][0].ToString());
                idKyThuTruoc = tblIDKyThuTruoc.Rows[0][0].ToString();
                string getPhieuThuTruoc = @"Select NgayThu from PhieuThu where IDKhachHang = " + pt.IDKhachHang
                    + @" and IDKyThu = " + idKyThuTruoc;
                DataTable tblPhieuThuTruoc = new DataTable();
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(getPhieuThuTruoc, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblPhieuThuTruoc.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                if (tblPhieuThuTruoc.Rows.Count > 0)
                {
                    if (string.IsNullOrEmpty(tblPhieuThuTruoc.Rows[0][0].ToString()))
                    {
                        return new JsonResult("Chưa đóng kỳ trước. Không thể đóng kỳ này");
                    }
                    else
                    {
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
                        return new JsonResult("Updated Successfully");
                    }
                }
                else
                {
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
                    return new JsonResult("Updated Successfully");
                    //return new JsonResult ("Không có phiếu thu trước");
                }
            }
            else
            {
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
                return new JsonResult("Updated Successfully");
                //return new JsonResult("Không có kỳ thu trước");
            }
        }
    }
}
