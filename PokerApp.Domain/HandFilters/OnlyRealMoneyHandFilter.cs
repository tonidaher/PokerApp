using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp.Domain.HandFilters
{
    public class OnlyRealMoneyHandFilter : IHandFilter
    {

        public OnlyRealMoneyHandFilter()
        {

        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            return hands.Where(x => x.Hand.Tournament.IsRealMoney);
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.OnlyRealMoney;
        }

        public void UpdateFilter(params string[] parameter)
        {
            
        }
    }
}
