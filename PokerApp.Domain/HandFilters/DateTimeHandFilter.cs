using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace PokerApp.Domain.HandFilters
{
    public class DateTimeHandFilter : IHandFilter
    {
        private DateTime _dateTime;

        public DateTimeHandFilter(params string[] parameters)
        {
            _dateTime = DateTime.Parse(parameters[0]);
        }
        public IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands)
        {
            return hands.Where(x => x.Hand.TimeStamp >= _dateTime);
        }

        public Constants.FilterType GetFilterType()
        {
            return Constants.FilterType.DateTime;
        }

        public void UpdateFilter(params string[] parameters)
        {
            _dateTime = DateTime.Parse(parameters[0]);
        }
    }
}
