using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class Employee
    {
        [Key]

        public int EmployeeId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public int? PositionId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime HireDate { get; set; }

        // Навигационные свойства
        public Position Position { get; set; }
        public Department Department { get; set; }
        public virtual ICollection<EmployeeRate> EmployeeRates { get; set; }
        public ICollection<WorkHour> WorkHours { get; set; } // Добавьте это
      

    }
}