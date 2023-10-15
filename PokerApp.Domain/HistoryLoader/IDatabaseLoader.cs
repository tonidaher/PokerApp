using PokerApp.Domain.Dal;
using System.Collections.Generic;

namespace PokerApp.Domain.HistoryLoader
{
    public interface IDatabaseLoader
    {
        IList<CurrentPlayersDal> GetCurrentPlayers();
    }
}
