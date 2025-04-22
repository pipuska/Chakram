using System;
using System.Collections.Generic;

namespace Chakram.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }

    public class Position
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public string Description { get; set; }

        public ICollection<PositionRate> PositionRates { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }

    public class PositionRate
    {
        public int RateId { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }
    }

    public class Employee
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public DateTime HireDate { get; set; }
        public string BankAccount { get; set; }
        public string TIN { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<EmployeeRate> EmployeeRates { get; set; }
        public ICollection<WorkHour> WorkHours { get; set; }
        public ICollection<SalaryPayment> SalaryPayments { get; set; }
    }

    public class EmployeeRate
    {
        public int EmployeeRateId { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }

    public class WorkHour
    {
        public int RecordId { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal HoursWorked { get; set; }
        public bool IsApproved { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }

    public class SalaryPayment
    {
        public int PaymentId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }

    public class SystemUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}