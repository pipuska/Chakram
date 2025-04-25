using System;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class EmployeeInputModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Не более 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Не более 50 символов")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Не более 50 символов")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Должность обязательна")]
        public int PositionId { get; set; }

        [Required(ErrorMessage = "Отдел обязателен")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "Дата приёма обязательна")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Required(ErrorMessage = "Ставка обязательна")]
        [Range(0.01, 100000, ErrorMessage = "Должна быть между 0.01 и 100000")]
        public decimal Rate { get; set; }
    }
}