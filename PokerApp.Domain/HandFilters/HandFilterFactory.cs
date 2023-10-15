using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static PokerApp.Domain.Common.Constants;

namespace PokerApp.Domain.HandFilters
{
    public class HandFilterFactory : IHandFilterFactory
    {
        private List<IHandFilter> _filters;
        private IHistoryHandContextProvider _context;

        public HandFilterFactory(IHistoryHandContextProvider context):this(new List<IHandFilter>(),context)
        {
            
        }
        public HandFilterFactory(List<IHandFilter> filters, IHistoryHandContextProvider context)
        {
            _filters = filters;
            _context = context;
        }

        public void AddFilter(FilterType filterType, params string[] parameters)
        {
            IHandFilter filter;
            switch (filterType)
            {
                case FilterType.CurrentSession:
                    filter = new CurrentSessionHandFilter(_context);
                    _filters.Add(filter);
                    break;
                case FilterType.GameType:
                    filter = _filters.FirstOrDefault(x => x is GameTypeHandFilter);
                    if (filter == null)
                    {
                        filter = new GameTypeHandFilter(parameters);
                        _filters.Add(filter);
                    }
                    else {
                        filter.UpdateFilter(parameters);
                    }
                    break;
                case FilterType.BuyIn:
                    filter = _filters.FirstOrDefault(x => x is BuyInHandFilter);
                    if (filter == null)
                    {
                        filter = new BuyInHandFilter(parameters);
                        _filters.Add(filter);
                    }
                    else
                    {
                        filter.UpdateFilter(parameters);
                    }
                    break;
                case FilterType.LastnHands:
                    filter = _filters.FirstOrDefault(x => x is LastNHandFilter);
                    if (filter == null)
                    {
                        filter = new LastNHandFilter(parameters);
                        _filters.Add(filter);
                    }
                    else
                    {
                        filter.UpdateFilter(parameters);
                    }
                    break;
                case FilterType.OnlyRealMoney:
                    filter = _filters.FirstOrDefault(x => x is OnlyRealMoneyHandFilter);
                    if (filter == null)
                    {
                        filter = new OnlyRealMoneyHandFilter();
                        _filters.Add(filter);
                    }
                    else
                    {
                        _filters.Remove(filter);
                    }
                    break;
                case FilterType.DateTime:
                    filter = _filters.FirstOrDefault(x => x is DateTimeHandFilter);
                    if (filter == null)
                    {
                        filter = new DateTimeHandFilter(parameters);
                        _filters.Add(filter);
                    }
                    else
                    {
                        filter.UpdateFilter(parameters);
                    }
                    break;

                default:
                    filter = new CurrentSessionHandFilter(_context);
                    _filters.Add(filter);
                    break;
            }
           
        }

        public IList<Join_PlayerHandDal> ApplyFilters(IList<Join_PlayerHandDal> hands)
        {
            if (!_filters.Any()){ return hands; }
            var result = hands.ToList();
            foreach (var filter in _filters)
            {
                result = filter.Filter(result).ToList();
            }
            return result;
        }

        public IEnumerator<IHandFilter> GetEnumerator()
        {
            return _filters.GetEnumerator();
        }

        public void RemoveFilter(FilterType filterType, params string[] parameters)
        {
            _filters.RemoveAll(x => x.GetFilterType().Equals(filterType));
        }

        public void ResetFilters()
        {
            _filters = new List<IHandFilter>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filters.GetEnumerator();
        }
    }
}
