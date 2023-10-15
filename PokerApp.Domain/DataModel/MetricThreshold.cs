
namespace PokerApp.Domain.DataModel
{
    public class MetricThreshold
    {
        public string MetricName { get; set; }
        public int MetricValue { get; set; }

        public int? Range { get; set; }

    }
}
