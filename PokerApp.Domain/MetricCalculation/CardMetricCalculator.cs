using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using PokerApp.Domain.HandFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tensorflow.Operations;

namespace PokerApp.Domain
{
    public class CardMetricCalculator : ICardMetricCalculator
    {
        private IHistoryHandContextProvider _contextProvider;
        public IHandFilterFactory FilterFactory { get; set; }

        public CardMetricCalculator(IHistoryHandContextProvider contextProvider, IHandFilterFactory filterFactory)
        {
            _contextProvider = contextProvider;
            FilterFactory = filterFactory;
        }

        public HandCardMetrics GetHandMetrics(string playerName, string cards)
        { if (cards.Length <= 2) return new HandCardMetrics(); 
            var labeledCards = LabelizeHand(cards);
            var context = _contextProvider.GetNewContext();
            var hands = context.GetPlayerHands(playerName);
            hands = FilterFactory.ApplyFilters(hands);
            var sameHands = hands.Where(x => LabelizeHand(x.Card1 + x.Card2) == labeledCards);

            return GetHandMetrics(labeledCards,sameHands) ;
        }

        private HandCardMetrics GetHandMetrics(string cards, IEnumerable<Join_PlayerHandDal> hands)
        {
            var stats = from p in hands
                        group p by 1 into g
                        select new
                        {
                            TotalCount = g.Count(),
                            WinCount = g.Count(x => x.EndStack > x.InitialStack),
                            RoiBBAmount = g.Sum(x => (x.EndStack - x.InitialStack) / x.Hand.BigBlind),
                            VpipCount = g.Count(x => (x.RaisePreflop || x.CallPreflop)),
                            VpipCountAndWin = g.Count(x => (x.RaisePreflop || x.CallPreflop) && (x.EndStack > x.InitialStack))
                        };

            var cardStats = stats.FirstOrDefault();
            if (cardStats == null)
            {
                return new HandCardMetrics
                {
                    Card = cards,
                };
            }

            return new HandCardMetrics
            {
                Card = cards,
                Count = cardStats.TotalCount,
                VpipCount = cardStats.VpipCount,
                RoiBBAmount = (int)cardStats.RoiBBAmount,
                WinCount = cardStats.WinCount,
                WinPercent = Percentage(cardStats.WinCount,cardStats.TotalCount),
                WinAndVpipPercent = Percentage(cardStats.VpipCountAndWin,cardStats.VpipCount)
            };

        }

        private int Percentage(int someCount, int totalCount)
        {
            if (totalCount == 0) return 0;
            return (int)(100.0 * someCount / totalCount);

        }
        private static Dictionary<char, byte> CardsRanking = new Dictionary<char, byte>()
        {
            { '2',1 },{ '3',2 },{ '4',3 },{ '5',4 },{ '6',5 }, { '7',6 },
            { '8',7 },{ '9',8 },{ 'T',9 },{ 'J',10 },{ 'Q',11 }, { 'K',12 },{ 'A',13 },
        };

        private string LabelizeHand(string cards)
        {
            var cleanCards = cards.Replace("10", "T");
            if (cleanCards.Length <= 2) return cleanCards;
            var orderedCards = OrderCards(cleanCards);

            var suit = GetSuitedLabel(orderedCards);

            return orderedCards[0].ToString() + orderedCards[2].ToString() + suit;

        }

        private string GetSuitedLabel(string cards)
        {
            if (cards.Length <= 2) return cards;
            if (cards[0] == cards[2])
            {
                return string.Empty;
            }
            if (cards[1] == cards[3])
            {
                return "s";
            }
            return "o";
        }

        private string OrderCards(string cards)
        {
            if (cards.Length <= 2) return cards;
            if (CardsRanking[cards[0]] > CardsRanking[cards[2]])
            {
                return string.Concat(cards[2], cards[3], cards[0], cards[1]);
            }
            return cards;
        }
    }
}
