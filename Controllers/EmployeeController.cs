using CrudPractice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CrudPractice.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IConfiguration configuration;

        public EmployeeController(IConfiguration config)
        {
            configuration = config;
        }

        //[HttpGet]
        #region select all employee
        public IActionResult Index(String empname = "", Double? sal = null, int deptid = 0, String cityname = "", DateTime? joiningdate=null)
        {
            LoadDepartmentDropDown();
            DataTable table = new DataTable();
            try
            {
                String connectingString = configuration.GetConnectionString("myConnection");
                using (SqlConnection con = new SqlConnection(connectingString))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_Employee_Search";


                        cmd.Parameters.Add("@EMPLOYEENAME", SqlDbType.VarChar).Value = empname??"";
                        cmd.Parameters.Add("@SALARY", SqlDbType.Decimal).Value = sal;
                        cmd.Parameters.Add("@DEPARTMENTID", SqlDbType.Int).Value = deptid;
                        cmd.Parameters.Add("@JOININGDATE", SqlDbType.Date).Value = joiningdate;
                        cmd.Parameters.Add("@CITY", SqlDbType.VarChar).Value = cityname??"";


                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        //SqlDataReader reader = cmd.ExecuteReader();
                        adapter.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading Employee data: " + ex.Message;
            }
            ViewBag.employeeName = empname; 
            ViewBag.salary = sal;
            ViewBag.deptID = deptid;
            ViewBag.joiningDate = joiningdate;
            ViewBag.cityname = cityname;

            return View("Index", table);
        }
        #endregion

        [HttpGet]
        #region delete employee
        public IActionResult Delete(int EmpID)
        {
            try
            {
                String connectingString = configuration.GetConnectionString("myConnection");
                using (SqlConnection conn = new SqlConnection(connectingString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_Employee_Delete";
                        cmd.Parameters.AddWithValue("@EmpID", EmpID);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Employee deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting Employee: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        #region add edit employee
        public IActionResult AddEdit(int? EmpID)
        {
            EmployeeModel model = new EmployeeModel();
            LoadDepartmentDropDown();
            if(EmpID != null)
            {
                try
                {
                    String connectingString = configuration.GetConnectionString("myConnection");
                    using (SqlConnection conn = new SqlConnection(connectingString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "PR_Employee_SelectByPK";
                            cmd.Parameters.AddWithValue("@EMPID", EmpID);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                model.EmpID = Convert.ToInt32(reader["EmpID"]);
                                model.EmpName = reader["EmpName"].ToString();
                                model.Salary = Convert.ToDouble(reader["Salary"]);
                                model.JoiningDate = Convert.ToDateTime(reader["JoiningDate"]);
                                model.City = reader["City"].ToString();
                                model.DeptID = Convert.ToInt32(reader["DeptID"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error loading Employee: " + ex.Message;
                }
            }
            return View("AddEdit", model);
        }
        #endregion
        [HttpPost]
        #region save employee
        public IActionResult Save(EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    LoadDepartmentDropDown();
                    return View("AddEdit", model);
                }
                String connectingString = configuration.GetConnectionString("myConnection");
                using (SqlConnection conn = new SqlConnection(connectingString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if(model.EmpID == 0)
                        {
                            cmd.CommandText = "PR_Employee_Insert";
                        }
                        else
                        {
                            cmd.CommandText = "PR_Employee_Update";
                            cmd.Parameters.AddWithValue("@EMPID", model.EmpID);
                        }
                        
                        cmd.Parameters.AddWithValue("@EmpName", model.EmpName);
                        cmd.Parameters.AddWithValue("@Salary", model.Salary);
                        //cmd.Parameters.AddWithValue("@JoiningDate", model.JoiningDate=DateTime.Now);
                        cmd.Parameters.AddWithValue("@City", model.City);
                        cmd.Parameters.AddWithValue("@DeptID", model.DeptID);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = model.EmpID == 0 ?
                "Employee added successfully!" :
                "Employee updated successfully!";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving Employee: " + ex.Message;
                return RedirectToAction("AddEdit");
            }
        }
        #endregion

        #region Load Department DropDown
        public void LoadDepartmentDropDown(int selectedDepartmentid=0)
        {
            List<DepartmentDropDownModel> departmentList = new List<DepartmentDropDownModel>();
            String connectingString = configuration.GetConnectionString("myConnection");
            using(SqlConnection conn = new SqlConnection(connectingString))
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PR_Department_SelectAll";

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        DepartmentDropDownModel model = new DepartmentDropDownModel();
                        model.DeptID = Convert.ToInt32(reader["DeptID"]);
                        model.DepartmentName = reader["DepartmentName"].ToString();
                        
                        departmentList.Add(model);
                    }
                }
            }
            ViewBag.DepartmentList = departmentList;
            ViewBag.DeptID = selectedDepartmentid;
        }
        #endregion
    }
}
