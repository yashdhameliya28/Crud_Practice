using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CrudPractice.Models
{
    public class DepartmentModel 
    {

        public int DeptID { get; set; }
        [Required(ErrorMessage = "Department Name is required")]
        [StringLength(100, ErrorMessage = "Department Name cannot exceed 100 characters")]
        public string DepartmentName { get; set; }
    }
}
