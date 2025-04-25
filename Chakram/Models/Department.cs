using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}