using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp.Domain.HandFilters
{
    public class GameTypeHandFilter : IHandFilter
    {
        private List<string> _gameTypes;
        public GameTypeHandFilter(params string[] gameTypes)
        {
            _gameTypes = gameTypes.ToList();
        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            return hands.Where(x => _gameTypes.Contains(x.Hand.Tournament.TournamentType));
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.GameType;
        }

        public void UpdateFilter(params string[] gameTypes)
        {
           foreach(var gameType in gameTypes)
            {
                if (_gameTypes.Contains(gameType))
                {
                    _gameTypes.Remove(gameType);
                }
                else
                {
                    _gameTypes.Add(gameType);
                }
            }
        }
    }
}
