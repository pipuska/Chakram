using System;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class SalaryPayment
    {
        [Key]
        public int PaymentId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; } = 0;
        public decimal TaxDeduction { get; set; } = 0;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}