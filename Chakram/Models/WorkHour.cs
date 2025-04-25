using System;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class WorkHour
    {
        [Key]
        public int RecordId { get; set; } // Идентификатор рабочего часа
        public int EmployeeId { get; set; } // Внешний ключ на Employee
        public DateTime WorkDate { get; set; } // Дата
        public int HoursWorked { get; set; } // Часы работы
        public bool IsApproved { get; set; }
        public Employee Employee { get; set; }
    }
}