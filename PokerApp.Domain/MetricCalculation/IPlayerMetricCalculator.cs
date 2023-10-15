using PokerApp.Domain.DataModel;
using PokerApp.Domain.HandFilters;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
   public interface IPlayerMetricCalculator
    {
       IHandFilterFactory FilterFactory { get; set; }
       IList<PlayerMetrics> GetCurrentPlayers();

        IList<MetricThreshold> GetThresholds(bool recompute=false);

        IList<Cards> GetLastNCards(string playerName,int numberOfHands=20);
       
    }
}
