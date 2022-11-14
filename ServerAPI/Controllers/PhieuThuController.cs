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
            string query = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
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

        [HttpGet("info/{idHD}")]
        public JsonResult GetInfoByID(int idHD)
        {
            string queryCheckDaThu = @"Select * from HoaDon where NgayThu is not null and IDHoaDon = " + idHD;
            DataTable tblCheckNgayThu = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheckDaThu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblCheckNgayThu.Load(myReader);

                    myReader.Close();
                    //myCon.Close();
                }
                if (tblCheckNgayThu.Rows.Count > 0)
                {
                    //Hoá đơn đã thu
                    string queryCheckHinhThuc = "Select * from HDThanhToanTrucTiep where IDHoaDon = " + idHD;
                    DataTable tableCheckHinhThuc = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryCheckHinhThuc, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tableCheckHinhThuc.Load(myReader);

                        myReader.Close();
                    }
                    if(tableCheckHinhThuc.Rows.Count > 0)
                    {
                        //Hoá đơn thanh toán trực tiếp
                        string query = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                        KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi, 
                        KyThu.IDKyThu,KyThu.TenKyThu,KyThu.Thang,KyThu.Nam,HoaDon.NgayTao,
                        HoaDon.NgayThu,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia,
                        TuyenThu.IDTuyenThu, TuyenThu.TenTuyenThu,
                        NhanVien.IDNhanVien, NhanVien.HoTen, NhanVien.MaNhanVien, NhanVien.SoDienThoai,
                        QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                        XaPhuong.IDXaPhuong, XaPhuong.TenXaPhuong
                        from HoaDon 
                        inner join HDThanhToanTrucTiep on HDThanhToanTrucTiep.IDHoaDon = HoaDon.IDHoaDon
                        inner join TuyenThu on TuyenThu.IDTuyenThu = HDThanhToanTrucTiep.IDTuyenThu
                        inner join NhanVien on NhanVien.IDNhanVien = HDThanhToanTrucTiep.IDNhanVien
                        inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                        inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                        inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                        inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                        where HoaDon.IDHoaDon = " + idHD;

                        DataTable tblInfo = new DataTable();
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tblInfo.Load(myReader);

                            myReader.Close();
                            myCon.Close();
                        }
                        return new JsonResult(new
                        {
                            tinhTrang = "Thanh toán trực tiếp",
                            info = tblInfo
                        }
                        );

                    }
                    else
                    {
                        //Hoá đơn thanh toán online
                        string query = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                        KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi, 
                        KyThu.IDKyThu,KyThu.TenKyThu,KyThu.Thang,KyThu.Nam,HoaDon.NgayTao,
                        HoaDon.NgayThu,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia,
                        Account.Username, Account.SDT,
                        QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                        XaPhuong.IDXaPhuong, XaPhuong.TenXaPhuong
                        from HoaDon 
                        inner join HDThanhToanOnline on HDThanhToanOnline.IDHoaDon = HoaDon.IDHoaDon
                        inner join Account on HDThanhToanOnline.IDAccount = Account.IDAccount
                        inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                        inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                        inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                        inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                        where HoaDon.IDHoaDon = " + idHD;

                        DataTable tblInfo = new DataTable();
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tblInfo.Load(myReader);

                            myReader.Close();
                            myCon.Close();
                        }

                        return new JsonResult(new
                        {
                            tinhTrang = "Thanh toán online",
                            info = tblInfo
                        }
                        );
                    }
                }
                else
                {
                    //Hoá đơn chưa thu
                    string query = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                        KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi,
                        KyThu.IDKyThu,KyThu.TenKyThu,KyThu.Thang,KyThu.Nam,HoaDon.NgayTao,
                        HoaDon.NgayThu,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia,
                        QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                        XaPhuong.IDXaPhuong, XaPhuong.TenXaPhuong
                        from HoaDon 
                        inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                        inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                        inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                        inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                        where IDHoaDon = " + idHD;
                    DataTable tblInfo = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblInfo.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                    return new JsonResult(new
                    {
                        tinhTrang = "Chưa thu",
                        info = tblInfo
                    }
                    );
                }
            }
        }

        [HttpGet("{idQuanHuyen}/{idTuyenThu}/{idXaPhuong}/{idKyThu}/{idKhachHang}/{idLoaiKhachHang}/{idHinhThuc}/{idNhanVien}/{idTrangThai}")]
        public JsonResult GetByConditions(int idQuanHuyen, int idTuyenThu, int idXaPhuong,
            int idKyThu, int idKhachHang, int idLoaiKhachHang, int idHinhThuc, int idNhanVien, int idTrangThai)
        {
            string selectFromString = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi,
                KyThu.IDKyThu,KyThu.TenKyThu,KyThu.Thang,KyThu.Nam,HoaDon.NgayTao,
                HoaDon.NgayThu,XaPhuong.IDXaPhuong,XaPhuong.TenXaPhuong,QuanHuyen.IDQuanHuyen,
                QuanHuyen.TenQuanHuyen,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia 
                from HoaDon ";

            string joinString = @"
                inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang ";

            string selectString = "";
            if(idHinhThuc == 1)
            {
                //Hoa don thanh toan truc tuyen
                joinString = string.Concat(joinString,
                    @" inner join HDThanhToanTrucTiep on HDThanhToanTrucTiep.IDHoaDon = HoaDon.IDHoaDon  
                        inner join NhanVien on NhanVien.IDNhanVien = HDThanhToanTrucTiep.IDNhanVien 
                        inner join TuyenThu on HDThanhToanTrucTiep.IDTuyenThu = TuyenThu.IDTuyenThu ");
            }

            if(idHinhThuc == 2)
            {
                //Cac hoa don thanh toan online
                joinString = string.Concat(joinString, " inner join HDThanhToanOnline on " +
                    "HDThanhToanOnline.IDHoaDon = HoaDon.IDHoaDon ");
            }
            selectString = string.Concat(selectFromString, joinString);
            
            string whereString = "";
            bool isFirstCondition = true;
            string orderString = " order by HoaDon.IDHoaDon DESC ";
            string query = "";

            if(idQuanHuyen != -1)
            {
                if(isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where QuanHuyen.IDQuanHuyen = ", idQuanHuyen);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND QuanHuyen.IDQuanHuyen = ", idQuanHuyen);
                }
            }
            if (idTuyenThu != -1)
            {
                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where TuyenThu.IDTuyenThu = ", idTuyenThu);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND TuyenThu.IDTuyenThu = ", idTuyenThu);
                }
            }
            if (idXaPhuong != -1)
            {
                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where XaPhuong.IDXaPhuong = ", idXaPhuong);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND XaPhuong.IDXaPhuong = ", idXaPhuong);
                }
            }
            if (idKyThu != -1)
            {
                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where KyThu.IDKyThu = ", idKyThu);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND KyThu.IDKyThu = ", idKyThu);
                }
            }
            if (idKhachHang != -1)
            {
                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where KhachHang.IDKhachHang = ", idKhachHang);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND KhachHang.IDKhachHang = ", idKhachHang);
                }
            }
            if (idLoaiKhachHang != -1)
            {
                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where LoaiKhachHang.IDLoaiKhachHang = ", idLoaiKhachHang);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND LoaiKhachHang.IDLoaiKhachHang = ", idLoaiKhachHang);
                }
            }
            if(idHinhThuc == 1)
            {
                //Hinh thuc thanh toan truc tiep thì duyệt where của idNhanVien
                if (idNhanVien != -1)
                {
                    if (isFirstCondition)
                    {
                        whereString = string.Concat(whereString, " where NhanVien.IDNhanVien = ", idNhanVien);
                        isFirstCondition = false;
                    }
                    else
                    {
                        whereString = string.Concat(whereString, " AND NhanVien.IDNhanVien = ", idNhanVien);
                    }
                }
            }
            if (idTrangThai != -1)
            {
                string statusNgayThu = "";

                //Đã thu
                if(idTrangThai == 1)
                {
                    statusNgayThu = " not null";
                }

                //Chưa thu
                if(idTrangThai == 2)
                {
                    statusNgayThu = " null";
                }

                if (isFirstCondition)
                {
                    whereString = string.Concat(whereString, " where NgayThu is ", statusNgayThu);
                    isFirstCondition = false;
                }
                else
                {
                    whereString = string.Concat(whereString, " AND NgayThu is ", statusNgayThu);
                }
            }

            query = string.Concat(selectString, whereString, orderString);
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

        [HttpGet("nhanvien/{idNhanVien}/{idKyThu}/{idKhachHang}/{idLoaiKhachHang}/{idHinhThuc}/{idTrangThai}")]
        public JsonResult GetByEmpAndConditions(int idNhanVien, int idKyThu, int idKhachHang, 
            int idLoaiKhachHang, int idHinhThuc, int idTrangThai)
        {
            string selectFromString = @"select HoaDon.MaSoHoaDon, HoaDon.IDHoaDon,KhachHang.MaKhachHang,
                KhachHang.IDKhachHang,KhachHang.HoTenKH, KhachHang.DiaChi,
                KyThu.IDKyThu,KyThu.TenKyThu,KyThu.Thang,KyThu.Nam,HoaDon.NgayTao,
                HoaDon.NgayThu,XaPhuong.IDXaPhuong,XaPhuong.TenXaPhuong,QuanHuyen.IDQuanHuyen,
                QuanHuyen.TenQuanHuyen,LoaiKhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai,LoaiKhachHang.Gia 
                from HoaDon ";

            string joinString = @"
                inner join KhachHang on HoaDon.IDKhachHang = KhachHang.IDKhachHang
                inner join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
                inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join TuyenThu on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu 
                inner join PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                inner join NhanVien on NhanVien.IDNhanVien = PhanTuyen.IDNhanVien 
                inner join QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                inner join LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang ";

            string selectString = "";
            if (idHinhThuc == 1)
            {
                //Hoa don thanh toan truc tuyen
                joinString = string.Concat(joinString,
                    " inner join HDThanhToanTrucTiep on HDThanhToanTrucTiep.IDHoaDon = HoaDon.IDHoaDon");
            }

            if (idHinhThuc == 2)
            {
                //Cac hoa don thanh toan online
                joinString = string.Concat(joinString, " inner join HDThanhToanOnline on " +
                    "HDThanhToanOnline.IDHoaDon = HoaDon.IDHoaDon ");
            }
            selectString = string.Concat(selectFromString, joinString);

            string whereString = " where NhanVien.IDNhanVien = " + idNhanVien;
            string orderString = " order by HoaDon.IDHoaDon DESC ";
            string query = "";

            if (idKyThu != -1)
            {
                whereString = string.Concat(whereString, " AND KyThu.IDKyThu = ", idKyThu);
            }
            if (idKhachHang != -1)
            {
                whereString = string.Concat(whereString, " AND KhachHang.IDKhachHang = ", idKhachHang);
            }
            if (idLoaiKhachHang != -1)
            {
                whereString = string.Concat(whereString, " AND LoaiKhachHang.IDLoaiKhachHang = ", idLoaiKhachHang);
            }

            if (idTrangThai != -1)
            {
                string statusNgayThu = "";

                //Đã thu
                if (idTrangThai == 1)
                {
                    statusNgayThu = " not null";
                }

                //Chưa thu
                if (idTrangThai == 2)
                {
                    statusNgayThu = " null";
                }

                whereString = string.Concat(whereString, " AND NgayThu is ", statusNgayThu);
            }

            query = string.Concat(selectString, whereString, orderString);
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
            string query = @"select * from XaPhuong
                inner join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join TuyenThu on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                inner join PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
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
                from dbo.KhachHang 
                inner join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
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
            string checkPhieuThu = @"Select * from HoaDon where IDKhachHang = " + pt.IDKhachHang +
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
                    string getMaxIDPhieuQuery = "select max(IDHoaDon) + 1 from HoaDon";
                    string getGiaTien = @"select Gia from KhachHang
                        join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang
                        where IDKhachHang = " + pt.IDKhachHang;
                    string maxIDPhieu = "";
                    int gia = 0;
                    DataTable dt = new DataTable();
                    DataTable dtGia = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(getMaxIDPhieuQuery, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dt.Load(myReader);
                        myReader.Close();
                    }
                    using (SqlCommand myCommand = new SqlCommand(getGiaTien, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        dtGia.Load(myReader);
                        myReader.Close();
                    }

                    maxIDPhieu = dt.Rows[0][0].ToString();
                    gia = int.Parse(dtGia.Rows[0][0].ToString());
                    string maSoHoaDon = "PT" + maxIDPhieu + "MKH" + pt.IDKhachHang + "D" + DateTime.Today.ToString("ddMMyyyy");
                    string query = @"insert into HoaDon values ("+ maxIDPhieu + "," + pt.IDKhachHang + "," 
                        + pt.IDKyThu + @",'" + maSoHoaDon + @"',GETDATE(),null," + gia + ")";
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
            string queryCheck = @"select * from HoaDon where NgayThu is not null and IDHoaDon = " + id;
            string queryDel = @" DELETE FROM HoaDon WHERE IDHoaDon = " + id;
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                }
                if(table.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Không thể xoá hoá đơn đã thanh toán"
                    }
                    );
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(queryDel, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                        myCon.Close();
                    }
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Xoá hoá đơn thành công"
                    }
                    );
                }
            }
        }
        //Put
        [HttpPut]
        public JsonResult Put(PhieuThu pt)
        {
            string getNgayThangKyThuQuery = @"select Thang, Nam from HoaDon 
	            join KyThu on HoaDon.IDKyThu = KyThu.IDKyThu
	            where HoaDon.IDHoaDon = " + pt.IDPhieu;
            DataTable tableNgayThangKyThu = new DataTable();

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getNgayThangKyThuQuery, myCon))
                {
                    Console.WriteLine(getNgayThangKyThuQuery);
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
                string getPhieuThuTruoc = @"Select NgayThu from HoaDon where IDKhachHang = " + pt.IDKhachHang
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
                        return new JsonResult(new
                        {
                            severity = "warning",
                            message = "Chưa đóng kỳ trước. Không thể đóng kỳ này"
                        });
                    }
                    else
                    {
                        string getIDTuyenThu = @"Select TuyenThu.IDTuyenThu from TuyenThu
                        join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                        join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                        join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        where IDKhachHang = " + pt.IDKhachHang;
                        DataTable tableIDTuyenThu = new DataTable();
                        using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                        {
                            myCon.Open();
                            using (SqlCommand myCommand = new SqlCommand(getIDTuyenThu, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                tableIDTuyenThu.Load(myReader);
                                myReader.Close();
                                myCon.Close();
                            }
                        }
                        string IDTuyenThu = tableIDTuyenThu.Rows[0][0].ToString();
                        string queryUpdateHoaDon = @" 
                        UPDATE dbo.HoaDon SET NgayThu = sysdatetime() WHERE IDHoaDon = " + pt.IDPhieu;
                        string queryInsertHDThanhToanTrucTiep = @"Insert into HDThanhToanTrucTiep values(
                        " + pt.IDPhieu + ", " + pt.IDNhanVien + ", " + IDTuyenThu + ")";

                        using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                        {
                            myCon.Open();
                            using (SqlCommand myCommand = new SqlCommand(queryUpdateHoaDon, myCon))
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
                        }
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Xác nhận thu hoá đơn thành công"
                        });
                    }
                }
                else
                {
                    //Không có hoá đơn tháng trước.
                    string getIDTuyenThu = @"Select TuyenThu.IDTuyenThu from TuyenThu
                        join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                        join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                        join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        where IDKhachHang = " + pt.IDKhachHang;
                    DataTable tableIDTuyenThu = new DataTable();
                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();
                        using (SqlCommand myCommand = new SqlCommand(getIDTuyenThu, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tableIDTuyenThu.Load(myReader);
                            myReader.Close();
                            myCon.Close();
                        }
                    }
                    string IDTuyenThu = tableIDTuyenThu.Rows[0][0].ToString();
                    string queryUpdateHoaDon = @" 
                        UPDATE dbo.HoaDon SET NgayThu = sysdatetime() WHERE IDHoaDon = " + pt.IDPhieu;
                    string queryInsertHDThanhToanTrucTiep = @"Insert into HDThanhToanTrucTiep values(
                        " + pt.IDPhieu + ", " + pt.IDNhanVien + ", " + IDTuyenThu + ")";

                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();
                        using (SqlCommand myCommand = new SqlCommand(queryUpdateHoaDon, myCon))
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
                    }
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Xác nhận thu hoá đơn thành công"
                    });
                }
            }
            else
            {
                //Không có kỳ thu trước
                string getIDTuyenThu = @"Select TuyenThu.IDTuyenThu from TuyenThu
                        join ThuocTuyen on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
                        join XaPhuong on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                        join KhachHang on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                        where IDKhachHang = " + pt.IDKhachHang;
                DataTable tableIDTuyenThu = new DataTable();
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(getIDTuyenThu, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tableIDTuyenThu.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                string IDTuyenThu = tableIDTuyenThu.Rows[0][0].ToString();
                string queryUpdateHoaDon = @" 
                        UPDATE dbo.HoaDon SET NgayThu = sysdatetime() WHERE IDHoaDon = " + pt.IDPhieu;
                string queryInsertHDThanhToanTrucTiep = @"Insert into HDThanhToanTrucTiep values(
                        " + pt.IDPhieu + ", " + pt.IDNhanVien + ", " + IDTuyenThu + ")";

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(queryUpdateHoaDon, myCon))
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
                }
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Xác nhận thu hoá đơn thành công"
                });
            }
        }
    }
}
