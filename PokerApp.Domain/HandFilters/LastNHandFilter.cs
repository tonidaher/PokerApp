using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokerApp.Domain.HandFilters
{
    public class LastNHandFilter : IHandFilter
    {
        private int nbHands;
        public LastNHandFilter(params string[] parameters)
        {
            nbHands = int.Parse(parameters[0]);
        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            if (nbHands == 0) return hands;
            return hands.OrderByDescending(x => x.Hand.TimeStamp).Take(nbHands);
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.LastnHands;
        }

        public void UpdateFilter(params string[] parameter)
        {
            nbHands =int.Parse(parameter[0]);
        }
    }
}
