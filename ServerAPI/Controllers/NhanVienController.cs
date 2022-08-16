using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ServerAPI.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public NhanVienController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"SELECT * FROM dbo.NhanVien
                join NhanVienAccount on NhanVien.IDNhanVien = NhanVienAccount.IDNhanVien
                join Account on Account.IDAccount = NhanVienAccount.IDAccount";
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

        [HttpGet("{id}")]
        public JsonResult GetByID(int id)
        {
            string query = @"SELECT * FROM dbo.NhanVien WHERE IDNhanVien =" + id;
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

        [HttpGet("getlastempid")]
        public JsonResult GetLastEmpID()
        {
            string query = @"SELECT TOP 1 IDNhanVien FROM dbo.NhanVien ORDER BY IDNhanVien DESC";
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

        [HttpGet("quyen/{id}")]
        public JsonResult GetByQuyen(int id)
        {
            string query = @"select * from dbo.NhanVien 
                join dbo.PhanQuyen on NhanVien.IDNhanVien = PhanQuyen.IDNhanVien 
                where IDQuyen = " + id;
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

        [HttpGet("quanhuyen/{id}")]
        public JsonResult GetByQuanHuyen(int id)
        {
            string query = @"select * from dbo.NhanVien 
                join dbo.PhanTuyen on NhanVien.IDNhanVien = PhanTuyen.IDNhanVien 
                where IDQuanHuyen = " + id;
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

        [HttpGet("CheckTuyenThu/{idNV}")]
        public JsonResult GetCheckTuyenThu(int idNV)
        {
            string checkTuyenThuQuery = "Select NgayKetThuc from NhanVien " +
                "join dbo.PhanTuyen on NhanVien.IDNhanVien = PhanTuyen.IDNhanVien " +
                "where NgayKetThuc is null and NhanVien.IDNhanVien = " + idNV;
            Console.WriteLine(checkTuyenThuQuery);
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(checkTuyenThuQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    if (dataTable.Rows.Count > 0)
                    {
                        myCon.Close();
                        return new JsonResult(false);
                    }
                    else
                    {
                        return new JsonResult(true);
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult Post(NhanVien emp)
        {
            string formattedNgaySinh = emp.NgaySinh.ToString("yyyy-MM-dd");
            string queryInsertNhanVien = @"
                INSERT INTO dbo.NhanVien VALUES
                    (" + emp.IDNhanVien + ",'" + emp.MaNhanVien + "', N'" + emp.HoTen +
                    "', '" + emp.Email + "', N'" + emp.GioiTinh + "', '" + emp.SoDienThoai + "','"
                    + formattedNgaySinh + @"', N'" + emp.DiaChi + "', '" + emp.CCCD + "')";

            DateTime ngaySinh = emp.NgaySinh;
            int namSinh = int.Parse(ngaySinh.Year.ToString());
            int currentNam = int.Parse(DateTime.Now.Year.ToString());
            if (currentNam - namSinh < 18)
            {
                return new JsonResult(new
                {
                    severity = "warning",
                    message = "Ngày sinh của nhân viên không hợp lệ. Nhân viên phải ít nhất 18 tuổi"
                });
            }



            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryInsertNhanVien, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                }

                string queryGetIDAccount = @"Select max(IDAccount) + 1 from Account";
                DataTable tblGetIDAccount = new DataTable();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    tblGetIDAccount.Load(myReader);
                    myReader.Close();
                }
                string IDAccount = tblGetIDAccount.Rows[0][0].ToString();

                string queryInsertAccount = @"Insert into Account values 
                    (" + IDAccount + ", '" + emp.TaiKhoan + "', '123456', '" + emp.SoDienThoai +"', anomyous.png)";

                using (SqlCommand myCommand = new SqlCommand(queryInsertAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                string queryInsertNhanVienAccount = @"insert into NhanVienAccount values 
                    (" + IDAccount + ", " + emp.IDNhanVien + ")";
                using (SqlCommand myCommand = new SqlCommand(queryInsertNhanVienAccount, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                string queryInsertQuyenNhanVien = @"Insert into PhanQuyen values 
                    (" + emp.IDNhanVien + ", IDQuyen)";
                using (SqlCommand myCommand = new SqlCommand(queryInsertQuyenNhanVien, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }

                myCon.Close(); 
            }

            return new JsonResult(table);
        }
        [HttpPut]
        public JsonResult Put(NhanVien emp)
        {
            string query = @"
                UPDATE dbo.NhanVien SET
                HoTen = N'" + emp.HoTen + @"',
                Email = '" + emp.Email + @"',
                GioiTinh = N'" + emp.GioiTinh + @"',
                SoDienThoai = '" + emp.SoDienThoai + @"',
                NgaySinh = '" + emp.NgaySinh + @"',
                DiaChi = N'" + emp.DiaChi + @"',
                CCCD = '" + emp.CCCD + @"' 
                WHERE IDNhanVien = '" + emp.IDNhanVien + @"'
                ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            DateTime ngaySinh = emp.NgaySinh;
            int namSinh = int.Parse(ngaySinh.Year.ToString());
            int currentNam = int.Parse(DateTime.Now.Year.ToString());
            if(currentNam - namSinh < 18)
            {
                return new JsonResult(new
                {
                    severity = "warning",
                    message = "Ngày sinh của nhân viên không hợp lệ. Nhân viên phải ít nhất 18 tuổi"
                });
            }
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

            return new JsonResult(new
            {
                severity = "success",
                message = "Chỉnh sửa nhân viên thành công"
            });
        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                DELETE FROM dbo.PhanQuyen 
                WHERE IDNhanVien = '" + id + @"'
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

        [Route("SaveProfilePicture")]
        [HttpPost]
        public JsonResult SaveProfilePicture()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _webHostEnvironment.ContentRootPath + "/NhanVien_ProfilePictures/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
    }
}
