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
            string query = @"SELECT * FROM dbo.NhanVien";
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
            string query = @"
                INSERT INTO dbo.NhanVien
                (MaNhanVien, HoTen, Email, GioiTinh, SoDienThoai, NgaySinh, DiaChi, CCCD, TaiKhoan, MatKhau) 
                OUTPUT inserted.IDNhanVien
                VALUES
                ('" + emp.MaNhanVien + @"', N'" + emp.HoTen + @"', '" + emp.Email + @"', N'" + emp.GioiTinh + @"', '" + emp.SoDienThoai + @"',
                '" + formattedNgaySinh + @"', N'" + emp.DiaChi + @"', '" + emp.CCCD + @"', '" + emp.TaiKhoan + @"', '" + emp.MatKhau + @"')
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
        [HttpPut]
        public JsonResult Put(NhanVien emp)
        {
            string query = @"
                UPDATE dbo.NhanVien SET 
                MaNhanVien = '" + emp.MaNhanVien + @"',
                HoTen = N'" + emp.HoTen + @"',
                Email = '" + emp.Email + @"',
                GioiTinh = N'" + emp.GioiTinh + @"',
                SoDienThoai = '" + emp.SoDienThoai + @"',
                NgaySinh = '" + emp.NgaySinh + @"',
                DiaChi = N'" + emp.DiaChi + @"',
                CCCD = '" + emp.CCCD + @"',
                TaiKhoan = '" + emp.TaiKhoan + @"',
                MatKhau = '" + emp.MatKhau + @"' 
                WHERE IDNhanVien = '" + emp.IDNhanVien + @"'
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

            return new JsonResult("Updated Successfully");
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
