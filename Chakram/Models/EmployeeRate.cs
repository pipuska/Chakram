using System;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class EmployeeRate
    {
        [Key]
        public int EmployeeRateId { get; set; } // Идентификатор ставки
        public int EmployeeId { get; set; } // Внешний ключ на Employee
        public decimal Rate { get; set; } // Ставка
        public DateTime EffectiveFrom { get; set; } // Дата начала действия ставки

        public Employee Employee { get; set; } // Навигационное свойство
    }
}