using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static PokerApp.Domain.Common.Constants;

namespace PokerApp.Domain.HandFilters
{
    public interface IHandFilter
    {
        IEnumerable<Join_PlayerHandDal> Filter(IEnumerable<Join_PlayerHandDal> hands);
        FilterType GetFilterType();
        void UpdateFilter(params string[] parameter);
    }
}
