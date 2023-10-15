using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerApp.Domain.Dal
{

    [Table("MetricThreshold")]
    public class MetricThresholdDal
    {
        [Key]
        public string MetricName { get; set; }
        public int MetricValue { get; set; }
        public int? Range { get; set; }
    }
}
