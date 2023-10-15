using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokerAppUI
{
    public class MetricThresholdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string metricName = (string)parameter;
            var metric = PlayerHudManager.MetricThresholds[metricName];
            if (!int.TryParse(value.ToString(), out int statValue))
            {
                return new SolidColorBrush(Colors.Red);
            }
            if (metric.Range != null)
            {


                var minValue = metric.MetricValue - metric.Range;
                var maxValue = metric.MetricValue + metric.Range;


                return (minValue <= statValue && (statValue <= maxValue)) ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
            }
            return (statValue >= metric.MetricValue)? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
