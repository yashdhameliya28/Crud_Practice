using CrudPractice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CrudPractice.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IConfiguration configuration;

        public DepartmentController(IConfiguration config)
        {
            configuration = config;
        }

        [HttpGet]
        #region select all department
        public IActionResult Index()
        {
            DataTable dt = new DataTable();
            try
            {
            String connectingString = configuration.GetConnectionString("myConnection");
                using (SqlConnection connection = new SqlConnection(connectingString))
                {
                    connection.Open();
                    using(SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "PR_Department_SelectAll";

                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading departemnt data: " + ex.Message;
            }
            return View("Index", dt);
        }
        #endregion

        [HttpGet]
        #region delete department
        public IActionResult Delete(int DeptID)
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
                        cmd.CommandText = "PR_Department_Delete";
                        cmd.Parameters.AddWithValue("@DeptID", DeptID);

                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Department deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting Department: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
        #endregion

        [HttpGet]
        #region add edit department
        public IActionResult AddEdit(int? deptid)
        {
            DepartmentModel model = new DepartmentModel();

            if(deptid != null)
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
                            cmd.CommandText = "PR_Department_SelectByPK";
                            cmd.Parameters.AddWithValue("@DEPARTMENTID", deptid);

                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                model.DeptID = Convert.ToInt32(reader["DeptID"]);
                                model.DepartmentName = reader["DepartmentName"].ToString();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error loading Department: " + ex.Message;
                }
            }
            return View("AddEdit", model);
        }
        #endregion

        [HttpPost]
        #region save department
        public IActionResult Save(DepartmentModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("AddEdit", model);
                }
                String connectingString = configuration.GetConnectionString("myConnection");
                using (SqlConnection conn = new SqlConnection(connectingString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if(model.DeptID == 0)
                        {
                            cmd.CommandText = "PR_Department_Insert";
                        }
                        else
                        {
                            cmd.CommandText = "PR_Department_Update";
                            cmd.Parameters.AddWithValue("@DEPTID", model.DeptID);
                        }

                        cmd.Parameters.AddWithValue("@DEPTNAME", model.DepartmentName);
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["SuccessMessage"] = "Department saved successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error saving Department: " + ex.Message;
                return View("AddEdit", model);
            }
        }
        #endregion
    }
}
