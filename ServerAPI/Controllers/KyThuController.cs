using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KyThuController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KyThuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from dbo.KyThu ORDER BY nam ASC, thang ASC";
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

        [HttpGet("years")]
        public JsonResult GetYears()
        {
            string query = @"select distinct Nam from dbo.KyThu";
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
            string query = @"select * from dbo.KyThu where IDKyThu = " + id;
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

        [HttpGet("{thang}/{nam}")]
        public JsonResult GetByThangAndNam(int thang, int nam)
        {
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            if (nam == -1 && thang == -1)
            {
                string query = @"select IDKyThu, TenKyThu, Thang, Nam, convert(varchar, NgayTao, 103) as NgayTao from dbo.KyThu ORDER BY nam ASC, thang ASC";
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
            else if (nam == -1 && thang != -1)
            {
                string query = @"select IDKyThu, TenKyThu, Thang, Nam, convert(varchar, NgayTao, 103) as NgayTao from dbo.KyThu where thang=" + thang + @"ORDER BY nam ASC, thang ASC";
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
            else if (nam != -1 && thang == -1)
            {
                string query = @"select IDKyThu, TenKyThu, Thang, Nam, convert(varchar, NgayTao, 103) as NgayTao from dbo.KyThu where nam=" + nam + @" ORDER BY nam ASC, thang ASC";
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
            else //if(nam != -1 && thang != -1)
            {
                string query = @"select IDKyThu, TenKyThu, Thang, Nam, convert(varchar, NgayTao, 103) as NgayTao from dbo.KyThu where thang=" + thang +
                    @" and nam=" + nam + @" ORDER BY nam ASC, thang ASC";
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
        }



        [HttpPost()]
        public JsonResult Post(KyThu kt)
        {
            string queryCheck = "Select * from dbo.KyThu where Thang = " + kt.Thang + @" and Nam =" + kt.Nam;
            DataTable dt = new DataTable();

            string query = @"insert into dbo.KyThu values
                (N'Kỳ thu tháng " + kt.Thang + @" năm " + kt.Nam + @"','" + kt.Thang + @"','" + kt.Nam + @"', SYSDATETIME())";
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                }

                if (dt.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult("Đã tồn tại kỳ thu tháng " + kt.Thang + " năm " + kt.Nam);
                }
                else
                {
                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                        myCon.Close();
                        return new JsonResult("Added Successfully");
                    }
                }
            }

        }


        [HttpPost("{status}")]
        public JsonResult Post(KyThu kt, bool status)
        {
            string queryCheck = "Select * from dbo.KyThu where Thang = " + kt.Thang + @" and Nam =" + kt.Nam;
            DataTable dt = new DataTable();
            string queryCheckTuyenThuKhachHang = @"Select IDKhachHang, IDTuyenThu, KhachHang.IDLoaiKhachHang, Gia 
                from dbo.KhachHang 
                join LoaiKhachHang on LoaiKhachHang.IDLoaiKhachHang = KhachHang.IDLoaiKhachHang
                join dbo.XaPhuong on dbo.KhachHang.IDXaPhuong = dbo.XaPhuong.IDXaPhuong 
                join dbo.ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                where TrangThai = 1";
            string sqlDataSource = _configuration.GetConnectionString("DBCon");

            DataTable table = new DataTable();
            DataTable table2 = new DataTable();
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheck, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                }
                if (dt.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Đã tồn tại kỳ thu tháng " + kt.Thang + " năm " + kt.Nam
                    }
                    );
                }
                else
                {
                    Console.WriteLine(status);
                    if (status)
                    {
                        using (SqlCommand myCommand = new SqlCommand(queryCheckTuyenThuKhachHang, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            table2.Load(myReader);

                            myReader.Close();
                        }
                        for (int i = 0; i < table2.Rows.Count; i++)
                        {
                            if (string.IsNullOrEmpty(table2.Rows[i][1].ToString()))
                            {
                                return new JsonResult(new
                                {
                                    severity = "warning",
                                    message = "Tồn tại khách hàng chưa có tuyến thu."
                                }
                                );
                            }
                        }
                        string queryGetIDKyThu = @"Select max(IDKyThu) + 1 from KyThu";
                        DataTable tblGetIDKyThu = new DataTable();
                        using (SqlCommand myCommand = new SqlCommand(queryGetIDKyThu, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tblGetIDKyThu.Load(myReader);
                            myReader.Close();
                        }
                        string IDKyThuInsert = tblGetIDKyThu.Rows[0][0].ToString();
                        string query = @"insert into dbo.KyThu values(" + IDKyThuInsert +
                            ",N'Kỳ thu tháng " + kt.Thang + " năm " + kt.Nam +
                            "','" + kt.Thang + "','" + kt.Nam + "', SYSDATETIME())";
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                        }
                        Console.WriteLine("Tạo hoá đơn");
                        for (int i = 0; i < table2.Rows.Count; i++)
                        {
                            string getIDPhieuQuery = "select max(IDHoaDon) + 1 from HoaDon";
                            string maSoPhieu = "HD";
                            DataTable tableID = new DataTable();
                            using (SqlCommand myCommand = new SqlCommand(getIDPhieuQuery, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                tableID.Load(myReader);
                                myReader.Close();
                            }
                            string IDPhieu = tableID.Rows[0][0].ToString();
                            int IDKH = int.Parse(table2.Rows[i][0].ToString());
                            int IDTuyen = int.Parse(table2.Rows[i][1].ToString());
                            int giaTien = int.Parse(table2.Rows[i][3].ToString());

                            int IDMauSoPhieu = int.Parse(table2.Rows[i][2].ToString());
                            maSoPhieu = String.Concat(maSoPhieu, IDPhieu,
                                "MKH", IDKH, "D", DateTime.Today.ToString("ddMMyyyy"));
                            string query4 = @"insert into HoaDon values (" + IDPhieu + ","
                                + IDKH + @"," + IDKyThuInsert + @",'" + maSoPhieu +
                                @"',GETDATE(),null," + giaTien + ")";

                            using (SqlCommand myCommand = new SqlCommand(query4, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                myReader.Close();
                            }
                        }
                        myCon.Close();
                    }
                    else
                    {
                        string queryGetIDKyThu = @"Select max(IDKyThu) + 1 from KyThu";
                        DataTable tblGetIDKyThu = new DataTable();
                        using (SqlCommand myCommand = new SqlCommand(queryGetIDKyThu, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tblGetIDKyThu.Load(myReader);
                            myReader.Close();
                        }
                        string IDKyThuInsert = tblGetIDKyThu.Rows[0][0].ToString();
                        string query = @"insert into dbo.KyThu values(" + IDKyThuInsert +
                            ",N'Kỳ thu tháng " + kt.Thang + " năm " + kt.Nam +
                            "','" + kt.Thang + "','" + kt.Nam + "', SYSDATETIME())";
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                            myCon.Close();
                        }
                    }
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Thêm kỳ thu thành công"
                    }
                    );
                }
            }
        }

        [HttpPut()]
        public JsonResult Put(KyThu kt)
        {
            string queryCheckExist = "Select * from dbo.KyThu where Thang = " + kt.Thang + @" and Nam =" + kt.Nam;
            DataTable dt = new DataTable();
            string query = @"update dbo.KyThu set TenKyThu = N'Kỳ thu tháng " + kt.Thang + @" năm " + kt.Nam + @"',
                Thang = " + kt.Thang + @", Nam = " + kt.Nam + @" where idKyThu = " + kt.IDKyThu;
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(queryCheckExist, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                }
                if (dt.Rows.Count > 0)
                {
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = "Không thể chỉnh sửa. Đã tồn tại kỳ thu tháng " + kt.Thang + " năm " + kt.Nam
                    }
                    );
                }
                else
                {
                    string queryCheckHoaDon = @"select * from HoaDon where IDkyThu = " + kt.IDKyThu +
                        " and NgayThu is not null";
                    DataTable tblCheckHoaDon = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryCheckHoaDon, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblCheckHoaDon.Load(myReader);
                        myReader.Close();
                    }
                    if (tblCheckHoaDon.Rows.Count > 0)
                    {
                        myCon.Close();
                        return new JsonResult(new
                        {
                            severity = "warning",
                            message = "Không thể chỉnh sửa thông tin. Tồn tại hoá đơn của kỳ này đã được thanh toán"
                        }
                        );
                    }
                    else
                    {
                        using (SqlCommand myCommand = new SqlCommand(query, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                            myCon.Close();
                            return new JsonResult(new
                            {
                                severity = "success",
                                message = "Cập nhật thông tin kỳ thu thành công"
                            }
                            );
                        }
                    }
                }
            }
        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"delete from dbo.KyThu where IDKyThu = " + id;
            string query2 = @"Select * from dbo.HoaDon where NgayThu IS NOT NULL and IDKyThu = " + id;
            DataTable dataTable = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query2, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dataTable.Load(myReader);
                    myReader.Close();
                    if (dataTable.Rows.Count > 0)
                    {
                        return new JsonResult(new
                        {
                            severity = "warning",
                            message = "Tồn tại hoá đơn đã được thu trong kỳ thu này.Không thể xoá kỳ thu"
                        }
                        );
                    }
                    else
                    {
                        string query3 = "Delete from dbo.HoaDon where IDKyThu = " + id;
                        using (SqlCommand delPhieuThuCommand = new SqlCommand(query3, myCon))
                        {
                            myReader = delPhieuThuCommand.ExecuteReader();

                            myReader.Close();
                        }
                        using (SqlCommand delKyThuCommand = new SqlCommand(query, myCon))
                        {
                            myReader = delKyThuCommand.ExecuteReader();

                            myReader.Close();
                            myCon.Close();
                        }
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Xoá kỳ thu thành công"
                        }
                        );
                    }
                }
            }
        }
    }
}
