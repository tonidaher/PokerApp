using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PokerApp.Domain.HandFilters
{
    public class PositionHandFilter : IHandFilter
    {
        private List<string> _positions;
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            return hands.Where(x => _positions.Contains(GetPosition(x.SeatNumber, x.Hand.MaxPlayers, x.Hand.DealerPosition,x.IsSmallBlind,x.IsBigBlind))) ;
        }

        private string GetPosition(int seatNumber, int maxPlayers, int dealerPosition, bool isSmallBlind, bool isBigBlind)
        {
            throw new NotImplementedException();
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.PlayerPosition;
        }

        public void UpdateFilter(params string[] positions)
        {
            foreach (var position in positions)
            {
                if (_positions.Contains(position))
                {
                    _positions.Remove(position);
                }
                else
                {
                    _positions.Add(position);
                }
            }
        }
    }
}
