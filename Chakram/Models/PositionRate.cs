using System.ComponentModel.DataAnnotations;

namespace Chakram.Models
{
    public class PositionRate
    {
        [Key]
        public int RateId { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public int PositionId { get; set; }
        public Position Position { get; set; }
    }
}