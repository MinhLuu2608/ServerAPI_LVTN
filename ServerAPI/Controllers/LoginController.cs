using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from dbo.NhanVien ";
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


        [HttpGet("{username}")]
        public JsonResult GetIDNhanVienByUsername(string username)
        {
            string query = "select NhanVien.IDNhanVien from NhanVienAccount " +
                "join NhanVien on NhanVienAccount.IDNhanVien = NhanVien.IDNhanVien " +
                "join Account on NhanVienAccount.idAccount = Account.idAccount " +
                "where Username = '" + username + "'";
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
            if(table.Rows.Count > 0)
            {
                return new JsonResult(table.Rows[0][0].ToString());
            }
            else
            {
                return new JsonResult("-1");
            }
        }

        [HttpGet("getinfobyID/{idNV}")]
        public JsonResult getinfobyID(int idNV)
        {
            string query = "select * from NhanVien where IdNhanVien = " + idNV;
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

        
        [HttpGet("getmastership/{idNV}")]
        public JsonResult getmastership(int idNV)
        {
            string query = "select IDNhanVien, IDQuyen from PhanQuyen where IDNhanVien = " + idNV;
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

        [HttpGet("getquyen/{idNV}")]
        public JsonResult getquyen(int idNV)
        {
            string query = @"select TenQuyen from Quyen 
                join PhanQuyen on Quyen.IDQuyen = PhanQuyen.IDQuyen 
                join NhanVien on NhanVien.IDNhanVien = PhanQuyen.IDNhanVien 
                where NhanVien.idNhanVien = " + idNV;
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

        [HttpPost]
        public JsonResult PostLoginWebsite(Login login)
        {
            string query = @"select * from Account 
                join NhanVienAccount on NhanVienAccount.IDAccount = Account.IDAccount 
                join NhanVien on NhanVien.IDNhanVien = NhanVienAccount.IDNhanVien
                join PhanQuyen on NhanVien.IDNhanVien = PhanQuyen.IDNhanVien
                where Username = '" + login.Username + "' and [password] = '" + login.Password + @"' 
                and IDQuyen = 1";
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
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
                }
            }
            if(table.Rows.Count > 0)
            {
                return new JsonResult("Connect");
            }
            else
            {
                return new JsonResult("Username Not Valid");
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
                return new JsonResult(new
                {
                    severity = "success",
                    message = "OK"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    severity = "warning",
                    message = "Mật khẩu không đúng!"
                });
            }
        }
    }
}
