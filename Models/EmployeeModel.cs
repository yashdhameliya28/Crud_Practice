using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CrudPractice.Models
{
    public class EmployeeModel 
    {
        public int EmpID { get; set; }

        [Required(ErrorMessage = "Employee Name is required")]
        [StringLength(100, ErrorMessage = "Employee Name cannot exceed 100 characters")]
        public string EmpName { get; set; }
        
        [Required(ErrorMessage ="Salary is required")]
        public Double Salary { get; set; }

        public DateTime JoiningDate { get; set; }

        [Required(ErrorMessage ="City is required")]
        [StringLength(100, ErrorMessage = "City Name cannot exceed 100 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select a department")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid department")]
        public int DeptID { get; set; }            
    }

    public class DepartmentDropDownModel
    {
        public int DeptID { get; set; }
        public string DepartmentName { get; set; }
    }
}
