using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }
        public string? PositionName { get; set; }
        public string? Description { get; set; }

        public ICollection<PositionRate> PositionRates { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}