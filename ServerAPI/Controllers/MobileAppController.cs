using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ServerAPI.Models;
using System;

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

        [HttpGet("isAbleToPay/{idHoaDon}")]
        public JsonResult GetCheckPayment(int idHoaDon)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryGetIDKhachHang = "Select IDKhachHang from HoaDon where IDHoaDon = " + idHoaDon;
            DataTable tblGetIDKhachHang = new DataTable();

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDKhachHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblGetIDKhachHang.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            string IDKhachHang = tblGetIDKhachHang.Rows[0][0].ToString();

            string getNgayThangKyThuQuery = @"select Thang, Nam from HoaDon 
	            join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
	            where HoaDon.IDHoaDon = " + idHoaDon;
            DataTable tableNgayThangKyThu = new DataTable();
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
                idKyThuTruoc = tblIDKyThuTruoc.Rows[0][0].ToString();
                string getPhieuThuTruoc = @"Select NgayThu from HoaDon where IDKhachHang = " + IDKhachHang
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
                    //Tồn tại hoá đơn tháng trước
                    if (string.IsNullOrEmpty(tblPhieuThuTruoc.Rows[0][0].ToString()))
                    {
                        //Chưa đóng hoá đơn tháng trước
                        return new JsonResult(false);
                    }
                    else
                    {
                        //Đã đóng hoá đơn tháng trước
                        return new JsonResult(true);
                    }
                }
                else
                {
                    //Không có hoá đơn tháng trước.
                    return new JsonResult(true);
                }
            }
            else
            {
                //Không có kỳ thu trước
                return new JsonResult(true);
            }

        }

        [HttpGet("PaymentType/{idHoaDon}")]
        public JsonResult GetPaymentType(int idHoaDon)
        {
            string queryCheckDaThu = "Select * from HoaDon where NgayThu is null and IDHoaDon = " + idHoaDon;
            DataTable tblCheckDaThu = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheckDaThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckDaThu.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if (tblCheckDaThu.Rows.Count > 0)
            {
                return new JsonResult("Chưa thanh toán");
            }
            else
            {
                string queryGetPaymentOnline = "Select * from HoaDon " +
                    "join HDThanhToanOnline on HoaDon.IDHoaDon = HDThanhToanOnline.IDHoaDon " +
                    "where HoaDon.IDHoaDon = " + idHoaDon;
                DataTable tblGetPaymentOnline = new DataTable();
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(queryGetPaymentOnline, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblGetPaymentOnline.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                if(tblGetPaymentOnline.Rows.Count > 0)
                {
                    return new JsonResult("Thanh toán online");
                }
                else
                {
                    return new JsonResult("Thanh toán trực tiếp");
                }
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

        [HttpGet("getInfoAccount/{idAccount}")]
        public JsonResult getInfoAccount(int idAccount)
        {
            string query = "Select * from account where IDAccount = " + idAccount;
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

        [HttpGet("getEmpID/{idAccount}")]
        public JsonResult getIDNhanVien(int idAccount)
        {
            string query = "Select IDNhanVien from NhanVienAccount where IDAccount = " + idAccount;
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
                return new JsonResult(int.Parse(table.Rows[0][0].ToString()));
            }
            else
            {
                return new JsonResult(-1);
            }

        }

        [HttpGet("getEmpRole/{idNV}")]
        public JsonResult getEmpRole(int idNV)
        {
            string getQuyen1 = "Select IDQuyen from PhanQuyen where IDQuyen = 1 and IDNhanVien = " + idNV;
            string getQuyen2 = "Select IDQuyen from PhanQuyen where IDQuyen = 2 and IDNhanVien = " + idNV;
            string getQuyen3 = "Select IDQuyen from PhanQuyen where IDQuyen = 3 and IDNhanVien = " + idNV;

            DataTable tableQuyen1 = new DataTable();
            DataTable tableQuyen2 = new DataTable();
            DataTable tableQuyen3 = new DataTable();

            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getQuyen1, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tableQuyen1.Load(myReader);
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(getQuyen2, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tableQuyen2.Load(myReader);
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(getQuyen3, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tableQuyen3.Load(myReader);
                    myReader.Close();
                }
                myCon.Close();
            }
            if (tableQuyen1.Rows.Count > 0 && tableQuyen2.Rows.Count == 0 && tableQuyen3.Rows.Count == 0)
            {
                return new JsonResult("Quản trị");
            }
            if(tableQuyen2.Rows.Count > 0 && tableQuyen3.Rows.Count == 0)
            {
                return new JsonResult("Thu tiền");
            }
            if (tableQuyen2.Rows.Count == 0 && tableQuyen3.Rows.Count > 0)
            {
                return new JsonResult("Dịch vụ");
            }
            return new JsonResult("Thu tiền & Dịch vụ");
        }

        [HttpGet("getEmp/{idNhanVien}")]
        public JsonResult getNhanVienInfo(int idNhanVien)
        {
            string query = @"Select MaNhanVien, HoTen, Email, GioiTinh, SoDienThoai, 
                format(NgaySinh, 'dd/MM/yyyy') as NgaySinh, DiaChi, CCCD
                from NhanVien 
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
            return new JsonResult(table);
        }

        [HttpGet("getTuyenThu/{idNhanVien}")]
        public JsonResult getTuyenThuByIDNhanVien(int idNhanVien)
        {
            string query = @"Select distinct TuyenThu.IDTuyenThu, TenTuyenThu, TenQuanHuyen
				from PhanTuyen
                join TuyenThu on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
                join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                join QuanHuyen on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
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
            return new JsonResult(table);
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
                    (" + IDAccount + ", '" + username + "', '" + password + "', null, null, '" + sdt + "', 'anonymous.png')";
                using (SqlCommand myCommand = new SqlCommand(queryInsertAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Đăng ký thành công");
            }
        }

        [HttpGet("customerHoaDonInfo/{idHoaDon}")]
        public JsonResult getCustomerHoaDonInfo(int idHoaDon)
        {
            string queryGetHoaDon = @"Select HoaDon.IDHoaDon, HoaDon.MaSoHoaDon, 
                format(HoaDon.NgayTao, 'dd/MM/yyyy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yyyy') as NgayThu, 
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
                where HoaDon.IDHoaDon = " + idHoaDon + " order by Thang desc, Nam desc ";

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

        [HttpGet("empHoaDonInfo/{idHoaDon}")]
        public JsonResult getHDToEmpByIDAccountAndFilter(int idHoaDon)
        {

            string queryGetHoaDon = @"Select HoaDon.IDHoaDon, HoaDon.MaSoHoaDon, 
                format(HoaDon.NgayTao, 'dd/MM/yyyy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yyyy') as NgayThu, 
                KyThu.TenKyThu, KyThu.Thang, TuyenThu.IDTuyenThu, TuyenThu.TenTuyenThu,
				NhanVien.IDNhanVien, NhanVien.HoTen, NhanVien.SoDienThoai,
                KhachHang.MaKhachHang, KhachHang.HoTenKH, LoaiKhachHang.Gia, LoaiKhachHang.TenLoai,
                CONCAT(KhachHang.Diachi, N', Phường ' , TenXaPhuong, ', ' , TenQuanHuyen) as DiaChiKH
                from NhanVien
				join PhanTuyen on NhanVien.IDNhanVien = PhanTuyen.IDTuyenThu
				join TuyenThu on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
				join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
				join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
				join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong 
				join HoaDon on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu 
				join QuanHuyen on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang
                where HoaDon.IDHoaDon = " + idHoaDon + " ";

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

        [HttpGet("getHoaDon/{IDAccount}")]
        public JsonResult getHDToKHByIDAccount(int IDAccount)
        {
            string queryGetHoaDon = @"Select HoaDon.IDHoaDon, HoaDon.MaSoHoaDon, 
                format(HoaDon.NgayTao, 'dd/MM/yyyy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yyyy') as NgayThu, 
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
                where Account.IDAccount = " + IDAccount + " order by Thang desc, Nam desc ";

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

        [HttpGet("getEmpFilter/{IDNhanVien}/{filterType}")]
        public JsonResult getHDToEmpByIDAccountAndFilter1(int IDNhanVien, int filterType)
        {
            //Filter type = 0: Lọc theo kỳ thu
            //Filter type = 1: Lọc theo tuyến thu
            //Filter type = 2: Lọc theo khách hàng
            //Filter type = 3: Lọc theo chưa thu

            string selectWhereString = "";

            string orderString = " order by ";

            if (filterType == 0)
            {
                selectWhereString = @"Select IDKyThu, TenKyThu, Thang, Nam, 
                    format(NgayTao, 'dd/MM/yyyy') as NgayTao from KyThu";
                orderString = string.Concat(orderString, " Thang desc, Nam desc");
            }
            if (filterType == 1)
            {
                selectWhereString = @"Select TuyenThu.IDTuyenThu, MaTuyenThu, TenTuyenThu, 
                    format(NgayBatDau, 'dd/MM/yyyy') as NgayBatDau, format(NgayKetThuc, 'dd/MM/yyyy') as NgayKetThuc 
                    from TuyenThu
                    join PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                    where IDNhanVien = " + IDNhanVien;
                orderString = string.Concat(orderString, " TuyenThu.IDTuyenThu");
            }
            if (filterType == 2)
            {
                selectWhereString = @"Select IDKhachHang, MaKhachHang, HoTenKH, DiaChi, CCCD
                    from KhachHang 
                    join XaPhuong on XaPhuong.IDXaPhuong = KhachHang.IDXaPhuong
                    join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                    join TuyenThu on TuyenThu.IDTuyenThu = ThuocTuyen.IDTuyenThu
                    join PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                    where IDNhanVien = " + IDNhanVien;
                orderString = string.Concat(orderString, " KhachHang.IDKhachHang");
            }
            if (filterType == 3)
            {
                selectWhereString = @"Select HoaDon.IDHoaDon, HoaDon.MaSoHoaDon, 
                format(HoaDon.NgayTao, 'dd/MM/yyyy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yyyy') as NgayThu, 
                KyThu.TenKyThu, KyThu.Thang, TuyenThu.IDTuyenThu, TuyenThu.TenTuyenThu,
				NhanVien.IDNhanVien, NhanVien.HoTen, NhanVien.SoDienThoai,
                KhachHang.MaKhachHang, KhachHang.HoTenKH, LoaiKhachHang.Gia, LoaiKhachHang.TenLoai,
                CONCAT(KhachHang.Diachi, N', Phường ' , TenXaPhuong, ', ' , TenQuanHuyen) as DiaChiKH
                from NhanVien
				join PhanTuyen on NhanVien.IDNhanVien = PhanTuyen.IDNhanVien
				join TuyenThu on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
				join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
				join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
				join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong 
				join HoaDon on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu 
				join QuanHuyen on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang
                where NhanVien.IDNhanVien = " + IDNhanVien + " and NgayThu is null";
                orderString = string.Concat(orderString, " NgayThu");
            }

            string queryGetHoaDon = string.Concat(selectWhereString, orderString);
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

        [HttpGet("getEmpHoaDonFilter/{IDNhanVien}/{filterType}/{ID}")]
        public JsonResult getHDToEmpByIDAccountAndFilter2(int IDNhanVien, int filterType, int ID)
        {
            //Filter type = 0: Lọc theo kỳ thu
            //Filter type = 1: Lọc theo tuyến thu
            //Filter type = 2: Lọc theo khách hàng

            string selectWhereString = @"Select HoaDon.IDHoaDon, HoaDon.MaSoHoaDon, 
                format(HoaDon.NgayTao, 'dd/MM/yyyy') as NgayTao, format(HoaDon.NgayThu, 'dd/MM/yyyy') as NgayThu, 
                KyThu.TenKyThu, KyThu.Thang, TuyenThu.IDTuyenThu, TuyenThu.TenTuyenThu,
				NhanVien.IDNhanVien, NhanVien.HoTen, NhanVien.SoDienThoai,
                KhachHang.MaKhachHang, KhachHang.HoTenKH, LoaiKhachHang.Gia, LoaiKhachHang.TenLoai,
                CONCAT(KhachHang.Diachi, N', Phường ' , TenXaPhuong, ', ' , TenQuanHuyen) as DiaChiKH
                from NhanVien
				join PhanTuyen on NhanVien.IDNhanVien = PhanTuyen.IDNhanVien
				join TuyenThu on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
				join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
				join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
				join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong 
				join HoaDon on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu 
				join QuanHuyen on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang ";
            string whereString = " where NhanVien.IDNhanVien = " + IDNhanVien;
            string orderString = " order by NgayThu";

            if (filterType == 0)
            {
                whereString = string.Concat(whereString, " AND HoaDon.IDKyThu = " + ID);
            }
            if (filterType == 1)
            {
                whereString = string.Concat(whereString, " AND TuyenThu.IDTuyenThu = " + ID);
            }
            if (filterType == 2)
            {
                whereString = string.Concat(whereString, " AND HoaDon.IDKhachHang = " + ID);
            }

            string queryGetHoaDon = string.Concat(selectWhereString, whereString, orderString);
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

        [HttpGet("getEmpOrders/{IDNhanVien}/{filterType}")]
        public JsonResult getDHToEmpByIDNhanVienAndFilter(int IDNhanVien, int filterType)
        {
            //Filter type = 0: "Chờ xử lý"
            //Filter type = 1: "Đã tiếp nhận"
            //Filter type = 2: "Đã hoàn thành"
            // Filter type = 3: "Đã bị huỷ"

            string selectWhereString = @"Select DonHangDV.IDDonHang, MaDonHang, TenKhachHang, DiaChiKH, SoDienThoaiKH,
                format(NgayTao, 'dd/MM/yyyy') as NgayTao, format(NgayHen, 'dd/MM/yyyy') as NgayHen, BuoiHen,
                format(NgayThu, 'dd/MM/yyyy') as NgayThu, TinhTrangXuLy, Note, TongTienDH, 
                MaNhanVien, HoTen, SoDienThoai
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien";
            string whereString = " ";
            string orderString = " ";

            if (filterType == 0)
            {
                whereString = string.Concat(whereString, " where TinhTrangXuLy = N'Chờ xử lý' ");
                orderString = string.Concat(orderString, " order by NgayTao");
            }
            if (filterType == 1)
            {
                whereString = string.Concat(whereString, 
                    " where TinhTrangXuLy = N'Đã tiếp nhận' and NhanVien.IDNhanVien = " + IDNhanVien);
                orderString = string.Concat(orderString, " order by NgayHen");
            }
            if (filterType == 2)
            {
                whereString = string.Concat(whereString,
                    " where TinhTrangXuLy = N'Đã hoàn thành' and NhanVien.IDNhanVien = " + IDNhanVien);
                orderString = string.Concat(orderString, " order by DonHangDV.IDDonHang desc");
            }
            if (filterType == 3)
            {
                whereString = string.Concat(whereString,
                    " where TinhTrangXuLy = N'Đã bị huỷ' and NhanVien.IDNhanVien = " + IDNhanVien);
                orderString = string.Concat(orderString, " order by DonHangDV.IDDonHang desc");
            }

            string queryGetHoaDon = string.Concat(selectWhereString, whereString, orderString);
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

        [HttpGet("getOrderInfo/{idDH}")]
        public JsonResult getOrderInfo(int idDH)
        {
            //Filter type = 0: "Chờ xử lý"
            //Filter type = 1: "Đã tiếp nhận"
            //Filter type = 2: "Đã hoàn thành"
            // Filter type = 3: "Đã bị huỷ"

            string queryGetHoaDon = @"Select DonHangDV.IDDonHang, MaDonHang, TenKhachHang, DiaChiKH, SoDienThoaiKH,
                format(NgayTao, 'dd/MM/yyyy') as NgayTao, format(NgayHen, 'dd/MM/yyyy') as NgayHen, BuoiHen,
                format(NgayThu, 'dd/MM/yyyy') as NgayThu, TinhTrangXuLy, Note, TongTienDH, 
                MaNhanVien, HoTen, SoDienThoai
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien
                where DonHangDV.IDDonHang = " + idDH;
            
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

        [HttpGet("getCustomerOrders/{IDAccount}/{filterType}")]
        public JsonResult getDHToCusByIDAccountAndFilter(int IDAccount, int filterType)
        {
            //Filter type = 0: "Chờ xử lý"
            //Filter type = 1: "Đã tiếp nhận"
            //Filter type = 2: "Đã hoàn thành"
            // Filter type = 3: "Đã bị huỷ"

            string selectWhereString = @"Select DonHangDV.IDDonHang, MaDonHang, TenKhachHang, DiaChiKH, SoDienThoaiKH,
                format(NgayTao, 'dd/MM/yyyy') as NgayTao, format(NgayHen, 'dd/MM/yyyy') as NgayHen, BuoiHen,
                format(NgayThu, 'dd/MM/yyyy') as NgayThu, TinhTrangXuLy, Note, TongTienDH, 
                MaNhanVien, HoTen, SoDienThoai
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien";
            string whereString = " where IDAccount = " + IDAccount;
            string orderString = " ";

            if (filterType == 0)
            {
                whereString = string.Concat(whereString, " and TinhTrangXuLy = N'Chờ xử lý' ");
                orderString = string.Concat(orderString, " order by NgayTao");
            }
            if (filterType == 1)
            {
                whereString = string.Concat(whereString,
                    " and TinhTrangXuLy = N'Đã tiếp nhận'");
                orderString = string.Concat(orderString, " order by NgayHen");
            }
            if (filterType == 2)
            {
                whereString = string.Concat(whereString,
                    " and TinhTrangXuLy = N'Đã hoàn thành' ");
                orderString = string.Concat(orderString, " order by DonHangDV.IDDonHang desc");
            }
            if (filterType == 3)
            {
                whereString = string.Concat(whereString,
                    " and TinhTrangXuLy = N'Đã bị huỷ'");
                orderString = string.Concat(orderString, " order by DonHangDV.IDDonHang desc");
            }

            string queryGetHoaDon = string.Concat(selectWhereString, whereString, orderString);
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

        [HttpGet("getSearchOrder/{maDH}")]
        public JsonResult getDHToCusByIDAccountAndFilter(string maDH)
        {
            //Filter type = 0: "Chờ xử lý"
            //Filter type = 1: "Đã tiếp nhận"
            //Filter type = 2: "Đã hoàn thành"
            // Filter type = 3: "Đã bị huỷ"

            string selectWhereString = @"Select DonHangDV.IDDonHang, MaDonHang, TenKhachHang, DiaChiKH, SoDienThoaiKH,
                format(NgayTao, 'dd/MM/yyyy') as NgayTao, format(NgayHen, 'dd/MM/yyyy') as NgayHen, BuoiHen,
                format(NgayThu, 'dd/MM/yyyy') as NgayThu, TinhTrangXuLy, Note, TongTienDH, 
                MaNhanVien, HoTen, SoDienThoai
                from DonHangDV 
                full outer join ChiTietTiepNhanDonHang on DonHangDV.IDDonHang = ChiTietTiepNhanDonHang.IDDonHang
                full outer join NhanVien on NhanVien.IDNhanVien = ChiTietTiepNhanDonHang.IDNhanVien";
            string whereString = " where MaDonHang = '" + maDH + "'";
            string orderString = " ";

            string queryGetHoaDon = string.Concat(selectWhereString, whereString, orderString);
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

        [HttpGet("getOrdersServiceInfo/{IDDonHang}")]
        public JsonResult getDHDichVuInfoByID(int IDDonHang)
        {

            string query = @"Select DichVu.IDDichVu, TenDichVu, DonViTinh, DonGia, SoLuong, TongTienDV
                from DonHangDV 
                join ChiTietDonHang on DonHangDV.IDDonHang = ChiTietDonHang.IDDonHang
                join DichVu on DichVu.IDDichVu = ChiTietDonHang.IDDichVu
                where DonHangDV.IDDonHang = " + IDDonHang;
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

        [HttpPost("checkRepass")]
        public JsonResult PostCheckRepass(Account account)
        {
            string query = "Select * from account where IDAccount = " + account.IDAccount +
                " and password = '" + account.Password + "'";
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
            if (table.Rows.Count > 0)
            {
                return new JsonResult("OK");
            }
            else
            {
                return new JsonResult("Mật khẩu không đúng!");
            }
        }

        [HttpPost("confirm")]
        public JsonResult PostConfirm(HoaDon hoaDon)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryCheckDaThu = "Select * from HoaDon where NgayThu is not null and IDHoaDon = " + hoaDon.IDHoaDon;
            DataTable tblCheckDaThu = new DataTable();
            
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheckDaThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckDaThu.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            if (tblCheckDaThu.Rows.Count > 0)
            {
                return new JsonResult("Hoá đơn đã được thanh toán. Không thể thanh toán tiếp");
            }

            string queryUpdateNgayThu = "Update HoaDon set NgayThu = sysdatetime() where IDHoaDon = " + hoaDon.IDHoaDon;
            string queryInsertHDThanhToanTrucTiep = "Insert into HDThanhToanTrucTiep values" +
                "(" + hoaDon.IDHoaDon + ", " + hoaDon.IDNhanVien + ", " + hoaDon.IDTuyenThu + ")";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateNgayThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(queryInsertHDThanhToanTrucTiep, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Xác nhận thanh toán thành công.");
            }
        }

        [HttpPost("acceptOrder")]
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
                if (tblCheck.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult("Đơn hàng này đã được tiếp nhận. Hãy kiểm tra lại");
                }

                DateTime ngayHen = DateTime.Parse(dh.NgayHen);
                DateTime today = DateTime.Today;
                if (DateTime.Compare(ngayHen, today) < 0)
                {
                    myCon.Close();
                    return new JsonResult("Ngày Hẹn không thể nhỏ hơn ngày hiện tại");
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
                return new JsonResult("Tiếp nhận đơn hàng thành công.");
            }
        }

        [HttpPost("request/{idDV}/")]
        public JsonResult PostRequestService(DonHang donHang, int idDV)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryGetIDDH = "Select max(IDDonHang) + 1 from DonHangDV";
            DataTable tblGetIDDH = new DataTable();

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDDH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblGetIDDH.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            string idDH = tblGetIDDH.Rows[0][0].ToString();
            string maDH = "DH";
            int SoMaxIDDH = 4 - idDH.Length;

            for (int i = 0; i < (SoMaxIDDH); i++)
            {
                maDH = String.Concat(maDH, "0");
            }
            maDH = String.Concat(maDH, idDH);

            string queryGetDichVu = "Select * from DichVu where IDDichVu = " + idDV;
            DataTable tblGetDichVu = new DataTable();
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetDichVu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblGetDichVu.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            string donGiaDV = tblGetDichVu.Rows[0]["DonGiaDV"].ToString();

            string queryInsertDH = "insert into dbo.DonHangDV values(" + idDH + ", " + donHang.IDAccount + ", '" 
                + maDH + "', N'" + donHang.TenKhachHang + "', N'" + donHang.DiaChiKH + "', '"
                + donHang.SoDienThoaiKH + "', sysdatetime(), null, N'', null, N'Chờ xử lý', N'', " + donGiaDV + ")";
            
            string queryInsertChiTietDH = "insert into dbo.ChiTietDonHang values(" + idDH + ", " + idDV + ", " 
                + donGiaDV + ", 1, " + donGiaDV + ")";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryInsertDH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(queryInsertChiTietDH, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
            }
            return new JsonResult("Yêu cầu dịch vụ thành công");
        }

        [HttpPut("editinfo")]
        public void PutEditAccountInfo(Account account)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryUpdateAccount = "Update Account set HoTenAccount = N'" + account.HoTen + 
                "', DiaChiAccount = N'" + account.DiaChi + "', SDT = '" + account.SDT + 
                "' where IDAccount = " + account.IDAccount;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
            }
        }

        [HttpPut("changePassword")]
        public JsonResult PutChangePassword(Account account)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryUpdateAccount = "Update Account set Password = N'" + account.Password + 
                "' where IDAccount = " + account.IDAccount;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
            }
            return new JsonResult("Thay đổi mật khẩu thành công");
        }

        [HttpPut("editOrder")]
        public JsonResult PutEditOrderInfo(DonHang donHang)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryUpdateInfoDonHang = "Update DonHangDV set TenKhachHang = N'" + donHang.TenKhachHang
                + "', DiaChiKH = N'" + donHang.DiaChiKH + "', SoDienThoaiKH = N'" + donHang.SoDienThoaiKH
                + "', TongTienDH = " + donHang.TongTienDH + " where IDDonHang = " + donHang.IDDonHang;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateInfoDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                for(int i=0; i < donHang.DichVuList.Length; i++)
                {
                    string queryUpdateChiTietDH = "Update ChiTietDonHang " +
                        "set SoLuong = " + donHang.DichVuList[i].SoLuong
                        + ", TongTienDV = " + donHang.DichVuList[i].TongTienDV + " where IDDonHang = "
                        + donHang.IDDonHang + " and IDDichVu = " + donHang.DichVuList[i].IDDichVu;
                    using (SqlCommand myCommand = new SqlCommand(queryUpdateChiTietDH, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }
                }
                myCon.Close();
                return new JsonResult("Chỉnh sửa đơn hàng thành công");
            }
        }

        [HttpPut("doneOrder")]
        public JsonResult PutDoneOrderInfo(DonHang donHang)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryUpdateInfoDonHang = "Update DonHangDV set TenKhachHang = N'" + donHang.TenKhachHang
                + "', DiaChiKH = N'" + donHang.DiaChiKH + "', SoDienThoaiKH = N'" + donHang.SoDienThoaiKH +
                "', TongTienDH = " + donHang.TongTienDH + ", NgayThu = sysdatetime(), TinhTrangXuLy = N'Đã hoàn thành'"
                + " where IDDonHang = " + donHang.IDDonHang;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateInfoDonHang, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                for (int i = 0; i < donHang.DichVuList.Length; i++)
                {
                    string queryUpdateChiTietDH = "Update ChiTietDonHang " +
                        "set SoLuong = " + donHang.DichVuList[i].SoLuong
                        + ", TongTienDV = " + donHang.DichVuList[i].TongTienDV + " where IDDonHang = "
                        + donHang.IDDonHang + " and IDDichVu = " + donHang.DichVuList[i].IDDichVu;
                    using (SqlCommand myCommand = new SqlCommand(queryUpdateChiTietDH, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }
                }
                myCon.Close();
                return new JsonResult("Hoàn thành đơn hàng thành công");
            }
        }

        [HttpPut("payment")]
        public void PutOnlinePayment(OnlinePayment paymentInfo)
        {
            SqlDataReader myReader;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string queryUpdateDaThu = "Update HoaDon set NgayThu = sysdatetime() where IDHoaDon = " + paymentInfo.IDHoaDon;
            string queryInsertHDThanhToanOnline = "Insert into HDThanhToanOnline values" +
                "(" + paymentInfo.IDHoaDon + ", " + paymentInfo.IDAccount + ")";
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryUpdateDaThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                using (SqlCommand myCommand = new SqlCommand(queryInsertHDThanhToanOnline, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
            }        
        }

        [HttpDelete("delete/{idDH}")]
        public JsonResult DeleteCancelByKH(int idDH)
        {
            string queryCheck = "Select * from DonHangDV where TinhTrangXuLy = N'Chờ xử lý' and IDDonHang = " + idDH;
            DataTable tblCheck = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheck.Load(myReader);
                    myReader.Close();
                }
                if (tblCheck.Rows.Count == 0)
                {
                    return new JsonResult("Đơn hàng đã được xử lý không thể huỷ.");
                }
                string queryCancelChiTiet = "Delete from ChiTietDonHang where IDDonHang = " + idDH;
                using (SqlCommand myCommand = new SqlCommand(queryCancelChiTiet, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                string queryCancel = "Delete from DonHangDV where IDDonHang = " + idDH;
                using (SqlCommand myCommand = new SqlCommand(queryCancel, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Đơn hàng được huỷ thành công");
            }
        }

        [HttpDelete("cancelByNV/")]
        public JsonResult DeleteCancelByNV(DonHang donHang)
        {
            string queryCheck = "Select * from DonHangDV where TinhTrangXuLy = N'Chờ xử lý' and IDDonHang = " + donHang.IDDonHang;
            DataTable tblCheck = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheck.Load(myReader);
                    myReader.Close();
                }

                string queryCheckTiepNhan = "Select * from ChiTietTiepNhanDonHang where IDDonHang = " + donHang.IDDonHang
                    + " and IDNhanVien = " + donHang.IDNhanVien;
                DataTable tblCheckTiepNhan = new DataTable();
                using (SqlCommand myCommand = new SqlCommand(queryCheckTiepNhan, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckTiepNhan.Load(myReader);
                    myReader.Close();
                }
                if (tblCheck.Rows.Count == 0 && tblCheckTiepNhan.Rows.Count == 0)
                {
                    return new JsonResult("Đơn hàng đã được xử lý hoặc bị huỷ bởi khách hàng hoặc nhân viên khác.");
                }


                string queryInsertTiepNhan = "Insert into ChiTietTiepNhanDonHang values(" + donHang.IDDonHang +
                    ", " + donHang.IDNhanVien + ")";
                string queryCancel = "Update DonHangDV set TinhTrangXuLy = N'Đã bị huỷ', Note = N'" + donHang.Note +
                    "' where IDDonHang = " + donHang.IDDonHang;

                if(tblCheckTiepNhan.Rows.Count == 0)
                {
                    using (SqlCommand myCommand = new SqlCommand(queryInsertTiepNhan, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }
                }
                using (SqlCommand myCommand = new SqlCommand(queryCancel, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult("Đơn hàng được huỷ thành công");
            }
        }
    }
}
