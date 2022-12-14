using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public KhachHangController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                select KhachHang.IDKhachHang, KhachHang.HoTenKH, KhachHang.MaKhachHang, KhachHang.CCCD, KhachHang.NgayTao, KhachHang.NgayKetThuc,
					KhachHang.DiaChi, KhachHang.IDXaPhuong, XaPhuong.TenXaPhuong, XaPhuong.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                    KhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai, KhachHang.TrangThai, PhanTuyen.IDNhanVien, ThuocTuyen.IDTuyenThu
                from KhachHang
                JOIN XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                JOIN QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                JOIN LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
				JOIN ThuocTuyen on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
				JOIN TuyenThu on TuyenThu.IDTuyenThu = ThuocTuyen.IDTuyenThu
				left OUTER JOIN PhanTuyen on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
				order by KhachHang.IDKhachHang, KhachHang.TrangThai desc
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
            myReader.Close();

            return new JsonResult(table);
        }

        [HttpGet("accountLinked/{idKH}")]
        public JsonResult GetAccountLinkedByIDKH(int idKH)
        {
            string query = @"Select Username from Account join LienKetTK on LienKetTK.IDAccount = Account.IDAccount
                where IDKhachHang = " + idKH;
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
                string accountLinkedString = "";
                for(int i=0; i< table.Rows.Count; i++)
                {
                    if(i != (table.Rows.Count - 1))
                        accountLinkedString = string.Concat(accountLinkedString, table.Rows[i][0].ToString(), " || ");
                    else
                        accountLinkedString = string.Concat(accountLinkedString, table.Rows[i][0].ToString());
                }
                return new JsonResult(accountLinkedString);
            }
            else
            {
                return new JsonResult("Chưa có tài khoản liên kết với ID khách hàng này");
            }

        }

        [HttpGet("getCustomerNoRoute")]
        public JsonResult GetCustomerNoRoute()
        {
            string query = @"
                select KhachHang.IDKhachHang, KhachHang.HoTenKH, KhachHang.MaKhachHang, KhachHang.CCCD, KhachHang.NgayTao, 
                    KhachHang.DiaChi, KhachHang.IDXaPhuong, XaPhuong.TenXaPhuong, XaPhuong.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                    KhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai, KhachHang.TrangThai, PhanTuyen.IDNhanVien, TuyenThu.IDTuyenThu
                from KhachHang
                JOIN XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                JOIN QuanHuyen on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                JOIN LoaiKhachHang on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
				left outer join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
				left outer join TuyenThu on TuyenThu.IDTuyenThu = ThuocTuyen.IDTuyenThu
				left OUTER JOIN PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
				where PhanTuyen.IDTuyenThu is null and KhachHang.TrangThai = 1
				order by QuanHuyen.IDQuanHuyen asc
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
            myReader.Close();

            return new JsonResult(table);
        }

        //lay thong tin khach hang tu idNhanVien
        [HttpGet("{idNhanVien}")]
        public JsonResult GetByStatus(int idNhanVien)
        {
            string query = @"
                select KhachHang.IDKhachHang, KhachHang.HoTenKH, KhachHang.MaKhachHang, KhachHang.CCCD, KhachHang.NgayTao, 
                    KhachHang.DiaChi, KhachHang.IDXaPhuong, XaPhuong.TenXaPhuong, XaPhuong.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                    KhachHang.IDLoaiKhachHang, LoaiKhachHang.TenLoai, KhachHang.TrangThai, PhanTuyen.IDNhanVien, TuyenThu.TenTuyenThu
                from KhachHang
                inner join XaPhuong
                on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join QuanHuyen
                on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen
                inner join LoaiKhachHang
				on KhachHang.IDLoaiKhachHang = LoaiKhachHang.IDLoaiKhachHang
                inner join ThuocTuyen 
				on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
				inner join TuyenThu 
				on ThuocTuyen.IDTuyenThu = TuyenThu.IDTuyenThu
				inner join PhanTuyen 
				on PhanTuyen.IDTuyenThu = TuyenThu.IDTuyenThu             
                where PhanTuyen.IDNhanVien = " + idNhanVien + "and KhachHang.TrangThai = 1" +
                "order by KhachHang.TrangThai desc"
            ;
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

        [HttpGet("quanhuyen/{idQuanHuyen}")]
        public JsonResult GetByQuanHuyen(int idQuanHuyen)
        {
            string selectString = @"
                select KhachHang.IDKhachHang, KhachHang.HoTenKH
                from KhachHang
                inner join XaPhuong
                on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
                inner join QuanHuyen
                on XaPhuong.IDQuanHuyen = QuanHuyen.IDQuanHuyen ";
            string whereString = " where QuanHuyen.IDQuanHuyen = " + idQuanHuyen;
            string orderString = " order by KhachHang.TrangThai desc";
            string query = "";
            if(idQuanHuyen == -1)
            {
                query = String.Concat(selectString, orderString);
            }
            else
            {
                query = String.Concat(selectString, whereString, orderString);
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
            myReader.Close();

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(KhachHang kh)
        {
            string maKH = "KH";

            string checkQuery = @"select * from KhachHang where CCCD = '" + kh.CCCD + "'";

            string getIDKyThuQuery = @"select IDKyThu from KyThu where Thang = Month(SYSDATETIME()) and Nam = Year(SYSDATETIME())";

            string getIDQuery = @"SELECT max(IDKhachHang) + 1 from KhachHang";

            string getIDTuyenThuQuery = @"select IDTuyenThu from ThuocTuyen where IDXaPhuong = " + kh.IDXaPhuong;

            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            DataTable checkCCCD = new DataTable();

            DataTable maxID = new DataTable();

            DataTable IDTuyenThu = new DataTable();

            DataTable maxIDKyThu = new DataTable();

            DataTable table = new DataTable();

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                                
                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    checkCCCD.Load(myReader);
                    myReader.Close();
                }

                using (SqlCommand myCommand = new SqlCommand(getIDTuyenThuQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    IDTuyenThu.Load(myReader);
                    myReader.Close();
                }

                if (IDTuyenThu.Rows.Count == 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Không Thể Thêm Khách Hàng Do Chưa tồn tại tuyến thu ở địa chỉ hiện tại"
                    });
                }
                else
                {
                    if (checkCCCD.Rows.Count == 0)
                    {
                        using (SqlCommand myCommand = new SqlCommand(getIDQuery, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            maxID.Load(myReader);
                            myReader.Close();
                        }
                 
                        string MaxIDKhachHang = maxID.Rows[0][0].ToString();
                        int SoMaxIDKhachHang = 6 - MaxIDKhachHang.Length;                     

                        for (int i = 0; i < (SoMaxIDKhachHang); i++)
                        {
                            maKH = String.Concat(maKH, "0");
                        }
                        maKH = String.Concat(maKH, MaxIDKhachHang);

                        string query = @"insert into dbo.KhachHang values
                            (" + MaxIDKhachHang + "," + kh.IDXaPhuong + @"," + kh.IDLoaiKhachHang + @",'" + maKH + @"',
                            N'" + kh.HoTenKH + @"',N'" + kh.DiaChi + @"','" + kh.CCCD + @"',GETDATE(),null,1)";
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                        }
                        myCon.Close();
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Thêm Khách Hàng Thành Công"
                        }
                        );
                    }
                    else
                    {
                        string queryCheckHoTen = @"select * from KhachHang where CCCD = '" + kh.CCCD + 
                            "' and HoTenKH = N'" + kh.HoTenKH + "'";
                        DataTable checkHoTen = new DataTable();
                        Console.WriteLine(queryCheckHoTen);
                        using (SqlCommand myCommand = new SqlCommand(queryCheckHoTen, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            checkHoTen.Load(myReader);
                            myReader.Close();
                        }
                        if(checkHoTen.Rows.Count > 0)
                        {
                            using (SqlCommand myCommand = new SqlCommand(getIDQuery, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                maxID.Load(myReader);
                                myReader.Close();
                            }

                            string MaxIDKhachHang = maxID.Rows[0][0].ToString();
                            int SoMaxIDKhachHang = 6 - MaxIDKhachHang.Length;

                            for (int i = 0; i < (SoMaxIDKhachHang); i++)
                            {
                                maKH = String.Concat(maKH, "0");
                            }
                            maKH = String.Concat(maKH, MaxIDKhachHang);

                            string query = @"insert into dbo.KhachHang values
                            (" + MaxIDKhachHang + "," + kh.IDXaPhuong + @"," + kh.IDLoaiKhachHang + @",'" + maKH + @"',
                            N'" + kh.HoTenKH + @"',N'" + kh.DiaChi + @"','" + kh.CCCD + @"',GETDATE(),null,1)";
                            using (SqlCommand myCommand = new SqlCommand(query, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                myReader.Close();
                            }
                            myCon.Close();
                            return new JsonResult(new
                            {
                                severity = "success",
                                message = "Thêm Khách Hàng Thành Công"
                            }
                            );
                        }
                        else
                        {
                            myCon.Close();
                            return new JsonResult(new
                            {
                                severity = "warning",
                                message = "CCCD đã tồn tại nhưng Họ tên KH không trùng với Họ tên của CCCD đó"
                            });
                        }
                    }                
                }              
            }
        }

        [HttpPut]
        public JsonResult Put(KhachHang kh)
        {
            string checkQuery = @"select * from KhachHang where CCCD = '" + kh.CCCD + "' and idKhachHang !="+ kh.IDKhachHang;

            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            DataTable checkCCCD = new DataTable();
             
            string query = @"update KhachHang 
                set IDXaPhuong = '" + kh.IDXaPhuong + "', HoTenKH = N'" + kh.HoTenKH + "'" +
                ", DiaChi = N'" + kh.DiaChi + "', CCCD = '" + kh.CCCD + "', IDLoaiKhachHang = " + kh.IDLoaiKhachHang  + 
                " where IDKhachHang =" + kh.IDKhachHang ;
            DataTable table = new DataTable();

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    checkCCCD.Load(myReader);
                    myReader.Close();
                }

                if (checkCCCD.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "CCCD đã tồn tại"
                    }
                        );
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        Console.WriteLine(query);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();

                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Chỉnh Sửa Khách Hàng Thành Công"
                        }
                        );
                    }
                }
            }
        }

        [HttpPut("customerNoRoute")]
        public JsonResult PutCustomerNoRoute()
        {
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            DataTable checkData = new DataTable();

            string checkQuery = @"select IDKhachHang from KhachHang join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong where IDTuyenThu is null and KhachHang.TrangThai = 1";

            string query = @"
                update KhachHang 
				set TrangThai = 0 
				where IDKhachHang in(
					select IDKhachHang from KhachHang
					join XaPhuong on KhachHang.IDXaPhuong = XaPhuong.IDXaPhuong
					where IDTuyenThu is null and KhachHang.TrangThai = 1)
            ";
            DataTable table = new DataTable();

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    checkData.Load(myReader);
                    myReader.Close();
                }

                if (checkData.Rows.Count == 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Không Tồn Tại Khách Hàng Không Có Tuyến Thu"
                    }
                        );
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        Console.WriteLine(query);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();

                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Cập Nhật Trạng Thái Khách Hàng Thành Công"
                        }
                        );
                    }
                }
            }
        }

        [HttpPut("{id},{status}")]
        public JsonResult PutStatus(int id, int status)
        {
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            string ngayKetThuc = "";

            if (status == 0)
            {
                ngayKetThuc = "sysdatetime()";
            }
            else if (status == 1) ngayKetThuc = "null";

            string query = @"update KhachHang set TrangThai = "+ status + 
                " , NgayKetThuc = " + ngayKetThuc + " where IDKhachHang =" + id;

            DataTable table = new DataTable();

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

                        if(status == 0)
                        {
                            return new JsonResult(new
                            {
                                severity = "success",
                                message = "Xoá Khách Hàng Thành Công"
                            }
                            );
                        }
                        else
                        {
                            return new JsonResult(new
                            {
                            severity = "success",
                            message = "Phục Hồi Khách Hàng Thành Công"
                            }
                            );
                    }
                        
                }
            }
        }
    }
}
