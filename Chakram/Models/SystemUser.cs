using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class SystemUser
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}