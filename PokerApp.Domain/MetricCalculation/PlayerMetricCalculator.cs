using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using PokerApp.Domain.HandFilters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApp.Domain
{
    public class PlayerMetricCalculator : IPlayerMetricCalculator
    {
        private IHistoryHandContextProvider _contextProvider;
        public IHandFilterFactory FilterFactory { get; set; }

        public List<MetricThreshold> MetricThresholds { get; set; }
        public PlayerMetricCalculator(IHistoryHandContextProvider contextProvider,IHandFilterFactory filterFactory)
        {
            _contextProvider = contextProvider;
            FilterFactory = filterFactory;
        }

        public IList<PlayerMetrics> GetCurrentPlayers()
        {
            var context =_contextProvider.GetNewContext();
            var currentPlayers = context.GetCurrentPlayers();
            var result = new List<PlayerMetrics>();
            foreach(var player in currentPlayers)
            {
                var hands  = context.GetPlayerHands(player.PlayerName);
                hands = FilterFactory.ApplyFilters(hands);
                result.Add(GetPlayerMetrics(player, hands));
            }
            return result;
        }

        private PlayerMetrics GetPlayerMetrics(CurrentPlayersDal player, IList<Join_PlayerHandDal> playerHands)
        {
          
            var handsCount = playerHands.Count;
                 
            var stats = from p in playerHands
            group p by 1 into g
            select new
            {
                PreFlopFoldCount= g.Count(x=>x.FoldPreflop),
                PreFlopCheckCount = g.Count(x => x.CheckPreflop),
                PreFlopCallCount = g.Count(x => x.CallPreflop),
                PreFlopRaiseCount = g.Count(x => x.BetPreflop || x.RaisePreflop),
                FlopFoldCount = g.Count(x => x.FoldFlop),
                FlopCheckCount = g.Count(x => x.CheckFlop),
                FlopCallCount = g.Count(x => x.CallFlop),
                FlopBetCount = g.Count(x => x.BetFlop),
                FlopRaiseCount = g.Count(x => x.RaiseFlop),
                TurnFoldCount = g.Count(x => x.FoldTurn),
                TurnCheckCount = g.Count(x => x.CheckTurn),
                TurnCallCount = g.Count(x => x.CallTurn),
                TurnBetCount = g.Count(x => x.BetTurn),
                TurnRaiseCount = g.Count(x => x.RaiseTurn),
                RiverFoldCount = g.Count(x => x.FoldRiver),
                RiverCheckCount = g.Count(x => x.CheckRiver),
                RiverCallCount = g.Count(x => x.CallRiver),
                RiverBetCount = g.Count(x => x.BetRiver),
                RiverRaiseCount = g.Count(x => x.RaiseRiver),
                SeeFlop = g.Count(x=>x.Seeflop),
                SeeTurn = g.Count(x=> x.SeeTurn),
                SeeRiver = g.Count(x=> x.SeeRiver),
                SeeShowdown = g.Count(x=>x.SeeShowdown),
                CBetFlopCount = g.Count(x=> (x.RaisePreflop || x.BetPreflop) && (x.BetFlop || x.RaiseFlop))
            };

            var playerStats = stats.FirstOrDefault();
            if (playerStats == null)
            {
                return new PlayerMetrics
                {
                    PlayerName = player.PlayerName,
                    CurrentTournamentId = player.TournamentId.ToString(),
                    CurrentTournamentName = player.TournamentName,
                    SeatNumber = player.SeatNumber,
                    HandsCount = handsCount
                };
            }
            var vpipCount = playerStats.PreFlopCallCount + playerStats.PreFlopRaiseCount;
            double betOrRaiseCount = playerStats.PreFlopRaiseCount + playerStats.FlopBetCount + playerStats.FlopRaiseCount + playerStats.TurnBetCount + playerStats.TurnRaiseCount + playerStats.RiverBetCount + playerStats.RiverRaiseCount;
            double callCount = playerStats.PreFlopCallCount + playerStats.FlopCallCount + playerStats.TurnCallCount + playerStats.RiverCallCount;

            return new PlayerMetrics
            {
                PlayerName = player.PlayerName,
                CurrentTournamentId = player.TournamentId.ToString(),
                CurrentTournamentName = player.TournamentName,
                SeatNumber= player.SeatNumber,
                HandsCount = handsCount,
                PreFlopFold = Percentage(playerStats.PreFlopFoldCount, handsCount),
                PreFlopRaise = Percentage(playerStats.PreFlopRaiseCount, handsCount),
                PreFlopCheck = Percentage(playerStats.PreFlopCheckCount, handsCount),
                PreFlopCall = Percentage(playerStats.PreFlopCallCount, handsCount),
                FlopFold = Percentage(playerStats.FlopFoldCount, playerStats.SeeFlop),
                FlopCheck = Percentage(playerStats.FlopCheckCount, playerStats.SeeFlop),
                FlopCall = Percentage(playerStats.FlopCallCount, playerStats.SeeFlop),
                FlopBet = Percentage(playerStats.FlopBetCount, playerStats.SeeFlop),
                FlopRaise = Percentage(playerStats.FlopRaiseCount, playerStats.SeeFlop),
                TurnFold = Percentage(playerStats.TurnFoldCount, playerStats.SeeTurn),
                TurnCheck = Percentage(playerStats.TurnCheckCount, playerStats.SeeTurn),
                TurnCall = Percentage(playerStats.TurnCallCount, playerStats.SeeTurn),
                TurnBet = Percentage(playerStats.TurnBetCount, playerStats.SeeTurn),
                TurnRaise = Percentage(playerStats.TurnRaiseCount, playerStats.SeeTurn),
                RiverFold = Percentage(playerStats.RiverFoldCount, playerStats.SeeRiver),
                RiverCheck = Percentage(playerStats.RiverCheckCount, playerStats.SeeRiver),
                RiverCall = Percentage(playerStats.RiverCallCount, playerStats.SeeRiver),
                RiverBet = Percentage(playerStats.RiverBetCount, playerStats.SeeRiver),
                RiverRaise = Percentage(playerStats.RiverRaiseCount, playerStats.SeeRiver),

                Vpip = Percentage(vpipCount, handsCount),
                Af = Math.Round(betOrRaiseCount / callCount, 1),
                InF= Percentage(playerStats.SeeFlop, handsCount),
                InT = Percentage(playerStats.SeeTurn, handsCount),
                InR = Percentage(playerStats.SeeRiver, handsCount),
                Cbet = Percentage(playerStats.CBetFlopCount,playerStats.PreFlopRaiseCount)
            };
        }

        private int Percentage(int someCount, int totalCount)
        {
            if (totalCount == 0) return 0;
            return (int)(100.0 * someCount / totalCount);
        }

        public IList<MetricThreshold> GetThresholds(bool recompute = false)
        {
            if (MetricThresholds == null)
            {
                var context = _contextProvider.GetNewContext();
                MetricThresholds = context.GetMetricThresholds().Select(x => new MetricThreshold() { MetricName = x.MetricName, MetricValue = x.MetricValue, Range = x.Range }).ToList();
            }
            return MetricThresholds;
        }

        public IList<Cards> GetLastNCards(string playerName, int numberOfHands=20)
        {
            var context = _contextProvider.GetNewContext();
            var hands = context.GetPlayerHands(playerName).Where(x => x.Card1 != null).ToList();
           return FilterFactory.ApplyFilters(hands).OrderByDescending(x => x.Hand.TimeStamp)
                .Take(numberOfHands).Select(x => new Cards(x.Card1, x.Card2, x.Card3, x.Card4)).ToList();
                
        }

    }
}
