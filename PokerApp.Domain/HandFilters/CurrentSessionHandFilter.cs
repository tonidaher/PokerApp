using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static PokerApp.Domain.Common.Constants;

namespace PokerApp.Domain.HandFilters
{
    public class CurrentSessionHandFilter : IHandFilter
    {
        private IHistoryHandContextProvider _contextProvider;
        public CurrentSessionHandFilter(IHistoryHandContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            //if (hands.Any(x => x.PlayerName == "metalingus87"))
            //{
                var context = _contextProvider.GetNewContext();
                var currentTournaments = context.GetCurrentPlayers().Select(x => x.TournamentId).Distinct();

                return hands.Where(x => currentTournaments.Contains(x.Hand.Tournament.TournamentId));
            //}
            //return hands;
        }

        public FilterType GetFilterType()
        {
            return FilterType.CurrentSession;
        }

        public void UpdateFilter(params string[] parameter)
        {
            
        }
    }
}
