using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;

using System.Collections.Generic;
using static PokerApp.Domain.Common.Constants;

namespace PokerApp.Domain.HandFilters
{
    public interface IHandFilterFactory : IEnumerable<IHandFilter>
    {
        IList<Join_PlayerHandDal> ApplyFilters(IList<Join_PlayerHandDal> hands);
        void AddFilter(FilterType filterType, params string[] parameters);
        void RemoveFilter(FilterType filterType, params string[] parameters);

        void ResetFilters();
    }
}
