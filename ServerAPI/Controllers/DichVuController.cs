using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DichVuController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public DichVuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("{loai}/{tinhTrang}")]
        public JsonResult GetDichVu(int loai, int tinhTrang)
        {
            //Loai = 1 => Trong nhà
            //Loai = 2 => Ngoài trời

            string selectQuery = "Select * from DichVu";
            string whereQuery = " where TinhTrangDV = " + tinhTrang;
            if(loai != -1)
            {
                if(loai == 1)
                {
                    whereQuery = string.Concat(whereQuery, " and LoaiDichVu = N'Trong nhà'");
                }
                else
                {
                    whereQuery = string.Concat(whereQuery, " and LoaiDichVu = N'Ngoài trời'");
                }
            }
            string query = string.Concat(selectQuery, whereQuery);
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

        [HttpPost]
        public JsonResult Post(Service service)
        {
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            string queryGetIDDichVu = "Select Max(IDDichVu) + 1 from DichVu";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryGetIDDichVu, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                }
                string idDV = table.Rows[0][0].ToString();
                try
                {
                    string queryInsert = "insert into dbo.DichVu values(" + idDV +
                        ", N'" + service.LoaiDichVu + "', N'" + service.TenDichVu + "', N'" + service.DonViTinh +
                        "', " + service.DonGiaDV + ",1)";
                    using (SqlCommand myCommand = new SqlCommand(queryInsert, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Thêm dịch vụ thành công"
                    });
                }
                catch (Exception ex)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Tên dịch vụ không thể trùng"
                    });
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpPut()]
        public JsonResult Put(Service service)
        {
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            string query = @"update DichVu set DonGiaDV = " + service.DonGiaDV +
                " where IDDichVu = " + service.IDDichVu;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    myReader.Close();
                }
                myCon.Close();
                return new JsonResult(new
                {
                    severity = "success",
                    message = "Chỉnh sửa dịch vụ thành công"
                }
                );
            }
        }

        [HttpPut("{id}/{status}")]
        public JsonResult PutStatus(int id, int status)
        {
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            string query = @"update DichVu set TinhTrangDV = " + status + 
                " where IDDichVu = " + id;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                if (status == 0)
                {
                    string queryCheck = @"Select * from ChiTietDonHang where IDDichVu = " + id;
                    DataTable tableCheck = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tableCheck.Load(myReader);
                        myReader.Close();
                    }
                    if(tableCheck.Rows.Count > 0)
                    {
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                        }
                        myCon.Close();  
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Ngừng cung cấp dịch vụ thành công"
                        });
                    }
                    else
                    {
                        string queryDelete = @"Delete from DichVu where IDDichVu = " + id;
                        using (SqlCommand myCommand = new SqlCommand(queryDelete, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                        }
                        myCon.Close();
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Xoá dịch vụ thành công"
                        });
                    }

                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Phục hồi dịch vụ thành công"
                    }
                    );
                }
            }
        }
    }
}
