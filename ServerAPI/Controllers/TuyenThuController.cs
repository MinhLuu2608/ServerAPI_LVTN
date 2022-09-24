using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace ServerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TuyenThuController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TuyenThuController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select * from dbo.TuyenThu ORDER BY TuyenThu.IDTuyenThu DESC";
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

        [HttpGet("allemp/{idTuyenThu}/{idNhanVien}")]
        public JsonResult GetAllEmpInTuyenThu(int idTuyenThu, int idNhanVien)
        {
            string query = @"
            select distinct HoTen,IDNhanVien from NhanVien where IDNhanVien in
            (
                select IDNhanVien from PhanTuyen
                where IDTuyenThu = " + idTuyenThu + @"
                union
                select distinct IDNhanVien from HDThanhToanTrucTiep
                where IDTuyenThu = " + idTuyenThu + @"
            )";
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
            string result = "";
            for(int i = 0; i < table.Rows.Count; i++)
            {
                if(i == table.Rows.Count - 1)
                {
                    if(idNhanVien == int.Parse(table.Rows[i][1].ToString()))
                    {
                        result = string.Concat(result, "ID:", table.Rows[i][1], ". Họ tên: ", table.Rows[i][0], " (Hiện tại)");
                    }
                    else
                    {
                        result = string.Concat(result, "ID:", table.Rows[i][1], ". Họ tên: ", table.Rows[i][0]);
                    }
                }
                else
                {
                    if (idNhanVien == int.Parse(table.Rows[i][1].ToString()))
                    {
                        result = string.Concat(result, "ID:", table.Rows[i][1], ". Họ tên: ", table.Rows[i][0], " (Hiện tại)\n");
                    }
                    else
                    {
                        result = string.Concat(result, "ID:", table.Rows[i][1], ". Họ tên: ", table.Rows[i][0], "\n");
                    }
                    
                }
            }
            return new JsonResult(result);
        }

        [HttpGet("{tinhTrang}/{nhanVien}/{quanHuyen}/{xaPhuong}")]
        public JsonResult GetByFilters(int tinhTrang, int nhanVien, int quanHuyen, int xaPhuong)
        {
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;
            if (tinhTrang != -1)
            {
                //Khong phai tuyen thu chua duoc trien khai
                string selectFromString = @"select distinct Tuyenthu.IDTuyenThu, Tuyenthu.MaTuyenThu, 
                    TuyenThu.TenTuyenThu, QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen, 
                    NhanVien.IDNhanVien, NhanVien.HoTen, 
                    convert(varchar, NgayBatDau, 103) as NgayBatDau, 
                    convert(varchar, NgayKetThuc, 103) as NgayKetThuc
                from dbo.TuyenThu 
                full outer join dbo.ThuocTuyen on TuyenThu.IDTuyenThu = ThuocTuyen.IDTuyenThu
                full outer join dbo.XaPhuong on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                full outer join dbo.PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu 
	            full outer join dbo.QuanHuyen on QuanHuyen.IDQuanHuyen = PhanTuyen.IDQuanHuyen 
                    or QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                full outer join dbo.NhanVien on PhanTuyen.IDNhanVien = NhanVien.IDNhanVien ";
                string whereString = @"where TuyenThu.IDTuyenThu is not null ";
                string orderByString = @"ORDER BY TuyenThu.IDTuyenThu DESC";

                if (nhanVien != -1)
                {
                    whereString = string.Concat(whereString, " AND NhanVien.IDNhanVien = ", nhanVien, " ");
                }
                if (quanHuyen != -1)
                {
                    whereString = string.Concat(whereString, " AND QuanHuyen.IDQuanHuyen = ", quanHuyen, " ");
                }
                if (xaPhuong != -1)
                {
                    string getTenXaPhuong = @"Select TenXaPhuong from XaPhuong where IDXaPhuong = " + xaPhuong;
                    DataTable tableTenXaPhuong = new DataTable();
                    using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                    {
                        myCon.Open();
                        using (SqlCommand myCommand = new SqlCommand(getTenXaPhuong, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            tableTenXaPhuong.Load(myReader);
                            myReader.Close();
                            myCon.Close();
                        }
                    }
                    string tenXaPhuong = tableTenXaPhuong.Rows[0][0].ToString();
                    whereString = string.Concat(whereString, " AND TuyenThu.TenTuyenThu like N'%", tenXaPhuong, "%' ");
                }

                if(tinhTrang == 0)
                {
                    //Tat ca tuyen thu
                    string query = string.Concat(selectFromString, whereString, orderByString);
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
                else
                {
                    if (tinhTrang == 1)
                    {
                        //Tuyen thu dang trien khai
                        whereString = string.Concat(whereString, " AND NgayBatDau is not null AND NgayKetThuc is null ");
                        string query = string.Concat(selectFromString, whereString, orderByString);
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
                    else
                    {
                        //Tuyen thu da ket thuc
                        whereString = string.Concat(whereString, " AND NgayKetThuc is not null ");
                        string query = string.Concat(selectFromString, whereString, orderByString);
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
                
            }
            else
            {
                //Tuyen thu chua duoc trien khai

                string selectFromString = @"select distinct Tuyenthu.IDTuyenThu, Tuyenthu.MaTuyenThu,
                    TuyenThu.TenTuyenThu, QuanHuyen.IDQuanHuyen, QuanHuyen.TenQuanHuyen,
                    NhanVien.IDNhanVien, NhanVien.HoTen,
                    convert(varchar, NgayBatDau, 103) as NgayBatDau,
                    convert(varchar, NgayKetThuc, 103) as NgayKetThuc
                    from dbo.TuyenThu
				    full outer join dbo.ThuocTuyen on TuyenThu.IDTuyenThu = ThuocTuyen.IDTuyenThu
                    full outer join dbo.XaPhuong on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                    full outer join dbo.PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu
                    full outer join dbo.QuanHuyen on QuanHuyen.IDQuanHuyen = PhanTuyen.IDQuanHuyen
                    or QuanHuyen.IDQuanHuyen = XaPhuong.IDQuanHuyen
                    full outer join dbo.NhanVien on PhanTuyen.IDNhanVien = NhanVien.IDNhanVien ";
                string whereString = @"where TuyenThu.IDTuyenThu is not null and NgayBatDau is null ";
                string orderByString = @"ORDER BY TuyenThu.IDTuyenThu DESC ";

                if (quanHuyen != -1)
                {
                    whereString = String.Concat(whereString, " AND QuanHuyen.IDQuanHuyen = ", quanHuyen, " ");
                }
                if (xaPhuong != -1)
                {
                    whereString = String.Concat(whereString, " AND XaPhuong.IDXaPhuong = ", xaPhuong, " ");
                }

                string query = String.Concat(selectFromString, whereString, orderByString);
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

        [HttpPost("{idQuanHuyen}")]
        public JsonResult Post(int idQuanHuyen, string[] xaPhuongChosenList)
        {
            string queryCheck = @"Select TenXaPhuong from dbo.XaPhuong 
                full outer join ThuocTuyen on ThuocTuyen.IDXaPhuong = XaPhuong.IDXaPhuong
                where IDTuyenThu is not null and (";
            string whereString = "";
            for(int i = 1; i <= xaPhuongChosenList.Length; i++)
            {
                if(i != xaPhuongChosenList.Length)
                {
                    whereString = string.Concat(whereString, "TenXaPhuong=N'", xaPhuongChosenList[i-1], "' or ");
                }
                else
                {
                    whereString = string.Concat(whereString, "TenXaPhuong=N'", xaPhuongChosenList[i-1], "')");
                }

            }
            queryCheck = string.Concat(queryCheck, whereString);

            DataTable dt = new DataTable();
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
                    string result = "Tồn tại xã phường đã có tuyến thu: ";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i != dt.Rows.Count - 1)
                        {
                            result = string.Concat(result, dt.Rows[i][0], ", ");
                        }
                        else
                        {
                            result = string.Concat(result, dt.Rows[i][0], ".");
                        }

                    }
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "warning",
                        message = result
                    }
                    );

                }
                else
                {
                    string tenQuanHuyen = "";
                    string queryGetTenQuanHuyen = "Select TenQuanHuyen from QuanHuyen " +
                        "where IDQuanHuyen = " + idQuanHuyen;
                    DataTable tblTenQuanHuyen = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryGetTenQuanHuyen, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblTenQuanHuyen.Load(myReader);
                        myReader.Close();
                    }
                    tenQuanHuyen = tblTenQuanHuyen.Rows[0][0].ToString();

                    //Lay Viet tat ten Quan Huyen
                    string subStr;
                    int countSpace = 0;
                    for (int i = 0; i < tenQuanHuyen.Length; i++)
                    {
                        subStr = tenQuanHuyen.Substring(i, 1);
                        if (subStr == " ")
                            countSpace++;
                    }
                    int[] spaceIndex = new int[countSpace];
                    int curSpaceIndex = -1;
                    string maTuyenThu = "";
                    for (int i = 0; i < spaceIndex.Length; i++)
                    {
                        spaceIndex[i] = tenQuanHuyen.IndexOf(" ", curSpaceIndex + 1);
                        curSpaceIndex = spaceIndex[i];
                        maTuyenThu = string.Concat(maTuyenThu, 
                            tenQuanHuyen.Substring(curSpaceIndex + 1, 1));
                    }

                    string queryGetMax = "Select cast(max(" +
                            "substring(MaTuyenThu," + (maTuyenThu.Length + 1) + @",50)) as INT) + 1" 
                        + "from TuyenThu where MaTuyenThu like N'%"+ maTuyenThu + @"%'";
                    DataTable maxMaTuyenThu = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryGetMax, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        maxMaTuyenThu.Load(myReader);
                        myReader.Close();
                    }
                    string maxID = maxMaTuyenThu.Rows[0][0].ToString();
                    if(maxID.Length == 0)
                    {
                        maxID = "1";
                    }
                    for(int i=0; i<=3-maxID.Length; i++)
                    {
                        maxID = string.Concat("0",maxID);
                    }
                    maTuyenThu = string.Concat(maTuyenThu, maxID);
                    string tenTuyen = string.Join(" - ", xaPhuongChosenList);

                    string queryGetIDTuyen = @"Select max(IDTuyenThu) + 1 from TuyenThu";
                    DataTable tblIDTuyenThu = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(queryGetIDTuyen, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblIDTuyenThu.Load(myReader);
                        myReader.Close();
                    }
                    string IDTuyenThu = tblIDTuyenThu.Rows[0][0].ToString();
                    //Insert Tuyen Thu
                    string queryInsertTT = "Insert into TuyenThu values("+ IDTuyenThu +",N'" + maTuyenThu + @"',N'"
                        + tenTuyen + @"')";
                    using (SqlCommand myCommand = new SqlCommand(queryInsertTT, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                    }

                    //Insert IDXaPhuong va IDTuyen thu vao trong dbo.ThuocTuyen
                    string queryInsert = "Insert into ThuocTuyen values(" + IDTuyenThu + ",";
                    string getIDXaPhuong = @"select IDXaPhuong from XaPhuong where ( " + whereString;
                    DataTable tblIDXaPhuong = new DataTable();
                    using (SqlCommand myCommand = new SqlCommand(getIDXaPhuong, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        tblIDXaPhuong.Load(myReader);
                        myReader.Close();
                    }
                    for(int i=0; i< tblIDXaPhuong.Rows.Count; i++)
                    {
                        string IDXaPhuong = tblIDXaPhuong.Rows[i][0].ToString();
                        string queryInsertThuocTuyen = queryInsert + IDXaPhuong + ")";
                        using (SqlCommand myCommand = new SqlCommand(queryInsertThuocTuyen, myCon))
                        {
                            myReader = myCommand.ExecuteReader();
                            myReader.Close();
                        }
                    }
                    myCon.Close();
                    return new JsonResult(new
                    {
                        severity = "success",
                        message = "Thêm tuyến thu thành công"
                    }
                    );
                }
            }
        }

        [HttpPut()]
        public JsonResult Put(PhanTuyen pt)
        {
            string querySelect = "Select NgayBatDau, NgayKetThuc from dbo.PhanTuyen where IDTuyenThu = " + pt.IDTuyenThu;
            string queryInsert = "Insert into dbo.PhanTuyen values(" + pt.IDNhanVien
                + @"," + pt.IDTuyenThu + @"," + pt.IDQuanHuyen
                + @", convert(varchar, SYSDATETIME(), 23) ,null)";

            string queryUpdate1 = @"update dbo.PhanTuyen set IDNhanVien = " + pt.IDNhanVien
                + @",  NgayBatDau = convert(varchar, SYSDATETIME(), 23)
                where IDTuyenThu = " + pt.IDTuyenThu;
            string queryUpdate2 = @"update dbo.PhanTuyen set IDNhanVien = " + pt.IDNhanVien
                + @" where IDTuyenThu = " + pt.IDTuyenThu;

            DataTable dt = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("DBCon");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(querySelect, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    dt.Load(myReader);
                    myReader.Close();
                }
                if (dt.Rows.Count > 0) 
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0][1].ToString()))
                    {
                        //Không thể update
                        myCon.Close();
                        return new JsonResult(new
                        {
                            severity = "warning",
                            message = "Tuyến thu này đã kết thúc. Không thể chỉnh sửa"
                        }
                        );
                    }
                    else
                    {
                        //Có thể Update
                        if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                        {
                            //Có Update NgayBD (Vì NgayBD Null)
                            using (SqlCommand myCommand = new SqlCommand(queryUpdate1, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                myReader.Close();
                                myCon.Close();
                                return new JsonResult(new
                                {
                                    severity = "success",
                                    message = "Cập nhật tuyến thu thành công"
                                }
                                );
                            }
                        }
                        else
                        {
                            //Không Update NgayBD
                            using (SqlCommand myCommand = new SqlCommand(queryUpdate2, myCon))
                            {
                                myReader = myCommand.ExecuteReader();
                                myReader.Close();
                                myCon.Close();
                                return new JsonResult(new
                                {
                                    severity = "success",
                                    message = "Cập nhật tuyến thu thành công"
                                }
                                );
                            }
                        }
                    }
                }
                else
                {
                    //Insert
                    using (SqlCommand myCommand = new SqlCommand(queryInsert, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Close();
                        myCon.Close();
                        return new JsonResult(new
                        {
                            severity = "success",
                            message = "Cập nhật tuyến thu thành công"
                        }
                        );
                    }
                }
            }
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query2 = @"Select * from dbo.TuyenThu
                full outer join dbo.PhanTuyen on TuyenThu.IDTuyenThu = PhanTuyen.IDTuyenThu 
                where NgayKetThuc is not null and TuyenThu.IDTuyenThu = " + id;

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
                            message = "Tuyến thu này đã kết thúc. Không thể thực hiện thao tác này"
                        }
                        );
                    }
                    else
                    {    
                        //Delete thuoc tuyen ( Giai phong XaPhuong khoi tuyen thu )
                        string queryDeleteTuyenXaPhuong = "delete from ThuocTuyen where IDTuyenThu = " + id;
                        using (SqlCommand getXaPhuongCMD = new SqlCommand(queryDeleteTuyenXaPhuong, myCon))
                        {
                            myReader = getXaPhuongCMD.ExecuteReader();
                            myReader.Close();
                        }

                        string queryCheckExist = "Select * from dbo.PhanTuyen where IDTuyenThu = " + id;
                        DataTable dt = new DataTable();
                        using (SqlCommand checkExistPhanTuyenCommand = new SqlCommand(queryCheckExist, myCon))
                        {
                            myReader = checkExistPhanTuyenCommand.ExecuteReader();
                            dt.Load(myReader);
                            myReader.Close();
                        }
                        if(dt.Rows.Count > 0)
                        {
                            //Ket thuc tuyen thu
                            string query = @"Update dbo.PhanTuyen 
                                set NgayKetThuc = convert(varchar, SYSDATETIME(), 23) 
                                where IDTuyenThu = " + id;
                            using (SqlCommand delTuyenThuCommand = new SqlCommand(query, myCon))
                            {
                                myReader = delTuyenThuCommand.ExecuteReader();
                                myReader.Close();
                                myCon.Close();
                            }
                            return new JsonResult(new
                            {
                                severity = "success",
                                message = "Tuyến thu đã được kết thúc thành công"
                            }
                            );
                        }
                        else
                        {
                            string queryCheckPhieuThu = "Select * from HDThanhToanTrucTiep where IDTuyenThu = " + id;
                            DataTable tableCheckPhieuThu = new DataTable();
                            using (SqlCommand checkPhieuThuCommand = new SqlCommand(queryCheckPhieuThu, myCon))
                            {
                                myReader = checkPhieuThuCommand.ExecuteReader();
                                tableCheckPhieuThu.Load(myReader);
                                myReader.Close();
                                
                            }
                            if(tableCheckPhieuThu.Rows.Count > 0)
                            {
                                myCon.Close();
                                return new JsonResult(new
                                {
                                    severity = "warning",
                                    message = "Không thể xoá tuyến thu vì tồn tại hoá đơn đã thanh toán thuộc tuyến này."
                                }
                                );
                            }
                            else
                            {
                                string query = @"Delete from dbo.TuyenThu where IDTuyenThu = " + id;
                                using (SqlCommand delTuyenThuCommand = new SqlCommand(query, myCon))
                                {
                                    myReader = delTuyenThuCommand.ExecuteReader();
                                    myReader.Close();
                                    myCon.Close();
                                }
                                return new JsonResult(new
                                {
                                    severity = "success",
                                    message = "Tuyến thu đã được kết thúc thành công"
                                }
                                );
                            }
                        }
                    }
                }
            }
        }
    }
}
