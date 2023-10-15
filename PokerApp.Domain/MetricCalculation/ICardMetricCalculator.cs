using PokerApp.Domain.DataModel;
using PokerApp.Domain.HandFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface ICardMetricCalculator
    {
        IHandFilterFactory FilterFactory { get; set; }

        HandCardMetrics GetHandMetrics(string playerName, string cards);

    }
}
