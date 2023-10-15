using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IHistoryHandContext
    {
        IList<Join_PlayerHandDal> GetPlayerHands(string player);
        IList<CurrentPlayersDal> GetCurrentPlayers();

        IList<TournamentDal> GetTournaments();

        IList<MetricThresholdDal> GetMetricThresholds();
    }
}
