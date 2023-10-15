using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp.Domain.HandFilters
{
    public class BuyInHandFilter : IHandFilter
    {

        private List<string> _buyIns;

        public BuyInHandFilter(params string[] buyIns)
        {
            _buyIns = buyIns.ToList();
        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            return hands.Where(x => _buyIns.Contains(x.Hand.Tournament.BuyIn));
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.BuyIn;
        }

        public void UpdateFilter(params string[] buyIns)
        {
            foreach (var buyIn in buyIns)
            {
                if (_buyIns.Contains(buyIn))
                {
                    _buyIns.Remove(buyIn);
                }
                else
                {
                    _buyIns.Add(buyIn);
                }
            }
        }
    }
}
