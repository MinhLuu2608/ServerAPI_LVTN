using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuanHuyenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public QuanHuyenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen from QuanHuyen";
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

        [HttpGet("AvailableDistricts")]
        public JsonResult GetAvailableDistricts()
        {
            string query = @"select distinct TenQuanHuyen, QuanHuyen.IDQuanHuyen from QuanHuyen
                join XaPhuong on QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
			    join ThuocTuyen on XaPhuong.IDXaPhuong = ThuocTuyen.IDXaPhuong
                where IDTuyenThu IS NOT NULL";
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

        [HttpGet("{idNhanVien}")]
        public JsonResult GetByStatus(int idNhanVien)
        {
            string query = @"
                select * from QuanHuyen
                join PhanTuyen on QuanHuyen.IDQuanHuyen = PhanTuyen.IDQuanHuyen
                where IDNhanVien =" + idNhanVien
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

        [HttpPost]
        public JsonResult Post(QuanHuyen qh)
        {

            string checkQuery = @"select * from QuanHuyen where TenQuanHuyen like N'" + qh.TenQuanHuyen + "'";

            DataTable checkDistrict = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    checkDistrict.Load(myReader);
                    myReader.Close();
                }

                if (checkDistrict.Rows.Count == 0)
                {
                    string queryGetIDQuanHuyen = "Select max(IDQuanHuyen) + 1 from QuanHuyen";
                    DataTable tblIDQuanHuyen = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryGetIDQuanHuyen, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblIDQuanHuyen.Load(myReader);
                        myReader.Close();
                    }
                    string IDQH = tblIDQuanHuyen.Rows[0][0].ToString();

                    string queryInsert = @"insert into QuanHuyen values
                        (" + IDQH + ",N'" + qh.TenQuanHuyen + "');";
                    using (SqlCommand myCommand = new SqlCommand(queryInsert, myCon))
                    {
                        myReader = myCommand.ExecuteReader();

                        myReader.Close();
                        myCon.Close();
                    }
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Thêm Quận Huyện Thành Công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Tên Quận Huyện Đã Tồn Tại"
                    });
                }

            }


        }

        [HttpDelete("{idQuanHuyen}")]
        public JsonResult Delete(int idQuanHuyen)
        {
            string query = @"delete QuanHuyen where IDQuanHuyen = " + idQuanHuyen;

            DataTable table = new DataTable();


            string checkQuery = @"select TenXaPhuong from XaPhuong where IDQuanHuyen =" + idQuanHuyen;

            DataTable checkDistrict = new DataTable();

            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(checkQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    checkDistrict.Load(myReader);
                    myReader.Close();
                }
                if (checkDistrict.Rows.Count == 0)
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();

                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Xoá Thành Công Quận Huyện"
                        });
                    }
                }
                else
                {
                    bool canDelete = true;
                    for (int i = 0; i < checkDistrict.Rows.Count; i++)
                    {
                        DataTable checkWard = new DataTable();
                        string Ward = @"select * from TuyenThu where TenTuyenThu like N'%" + checkDistrict.Rows[i][0] + "%'";

                        using (SqlCommand myCommand = new SqlCommand(Ward, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            checkWard.Load(myReader);
                            myReader.Close();
                        }
                        if (checkWard.Rows.Count != 0)
                        {
                            canDelete = false;
                        }
                    }
                    if (canDelete == true)
                    {
                        string deleleWards = "delete XaPhuong where IDQuanHuyen =" + idQuanHuyen;

                        using (SqlCommand myCommand = new SqlCommand(deleleWards, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            table.Load(myReader);
                            myReader.Close();
                        }

                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            table.Load(myReader);
                            myReader.Close();
                            myCon.Close();
                        }
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Xoá Thành Công Quận Huyện Và Xã Phường Thành Công"
                        });
                    }
                    else
                    {
                        return new JsonResult(new
                        {
                            severity = "warning",
                            message = "Không Thể Xoá Quận Huyện"
                        });
                    }
                }

            }
        }

    }
}
