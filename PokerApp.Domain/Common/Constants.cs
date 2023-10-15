
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PokerApp.Domain.Common
{
    public class Constants
    {
        public const string None = "none";

        public class FilePattern
        {
            public const string FileNameExample = @"20200802_Kill The Fish(378055975)_real_holdem_no-limit.txt";
            public const string FilaNamePattern = @"(?<date>[0-9]+)_(?<name>.*)_(?<isreal>real|play)_(?<type>.*).txt";

            public const string HandBeginTournamentExample = @"Winamax Poker - Tournament ""Monster Stack"" buyIn: 0.22€ + 0.03€ level: 3 - HandId: #1635014429483466756-57-1597620371 - Holdem no limit (160/700/1400) - 2020/08/16 23:26:11 UTC";
            public const string HandBeginTournamentPattern = @"(?<PokerSite>Winamax Poker) - Tournament [""].*[""] buyIn: (?<BuyIn>[0-9.€]+ [+] [0-9.€]+) level: (?<HandLevel>[0-9]+) - HandId: #(?<HandId>[0-9-]+) - [\w -/]+ \((?<Blind1>[0-9]+)/(?<Blind2>[0-9]+)/?(?<Blind3>[0-9]*)\) - (?<TimeStamp>[0-9/: ]+) UTC";

            public const string HandBeginCashGameExample = @"Winamax Poker - CashGame - HandId: #14835299-376-1597697086 - Holdem no limit (0.01/0.02) - 2020/08/17 20:44:46 UTC";
            public const string HandBeginCashGamePattern = @"(?<PokerSite>Winamax Poker) - CashGame - HandId: #(?<HandId>[0-9-]+) - [\w ]+ \((?<Blind1>[0-9.]+)/(?<Blind2>[0-9.]+)/?(?<Blind3>[0-9.]*)\) - (?<TimeStamp>[0-9/: ]+) UTC";

            public const string PlayerSeatedExample = @"Seat 1: flushalas47 (2)";
            public const string PlayerSeatedPattern = @"Seat (?<SeatNumber>[0-9]+): (?<PlayerName>.*) \((?<InitialStack>[0-9.]+)\)";

            public const string MaxPlayersPattern = @"(?<MaxPlayers>\d+)-max";
            //ASTRE31620 posts ante 10
            //lion1400 posts big blind 300
            //lion1400 posts small blind 150
            //metalingus87 raises 1290 to 1500 and is all-in
            //jibjib313 calls 135
            //ropinam folds

            public const string HandActionPattern = @"(?<PlayerName>#Players#) (?<ActionName>shows|collected|calls|bets|checks|raises|folds|posts ante|posts big blind|posts small blind)[ ]?(\[(?<Cards>[a-zA-Z0-9 ]+)\])?(?<Amount>[0-9.]+)?( to (?<PotAmount>[0-9.]+))?(?<IsAllIn> and is all-in)?";
            public const string PlayerDealtPattern = @"Dealt to (?<PlayerName>#Players#) (\[(?<Cards>[a-zA-Z0-9 ]+)\])";

            public const string PlayersVariable = @"#Players#";
        }
        public class PlayerPosition
        {
            public const string Dealer = "Dealer";
            public const string Utg = "UTG";
            public const string CutOff = "Cut Off";
            public const string HighJack = "High Jack";
            public const string SmallBlind = "Small Blind";
            public const string BigBlind = "Big Blind";
            public const string MidPosition = "Mid Position";
        }

        public class FileHeaders
        {
            public const string HandBegin = "Winamax Poker";
            public const string TableBegin = "Table:";
            public const string AnteBlinds = "*** ANTE/BLINDS ***";
            public const string Preflop = "*** PRE-FLOP ***";
            public const string Flop = "*** FLOP ***";
            public const string Turn = "*** TURN ***";
            public const string River = "*** RIVER ***";
            public const string Showdown = "*** SHOW DOWN ***";
            public const string Summary = "*** SUMMARY ***";
            public const string Dealt = "Dealt";
            public const string TotalPot = "Total pot";
            public const string Board = "Board";
            public const string Seat = "Seat";

        }

        public class TournamentIndicator
        {
            public const string Tournament = "Tournament";
            public const string CashGame = "CashGame";
        }

       // public static Dictionary<string, short> ActionTypes = InitActionTypes();
        public static Dictionary<string, Dim_ActionDal> Actions = InitActions();
        public static Dictionary<string, short> Steps = InitSteps();

        public class HandSteps {
            public const string AnteBlinds = "AnteBlinds";
            public const string PreFlop = "PreFlop";
            public const string Flop = "Flop";
            public const string Turn = "Turn";
            public const string River = "River";
            public const string Showdown = "Showdown";
        }

        public class PokerPostion
        {
            //Early
            public const string SmallBlind = "Small Blind";
            public const string BigBlind = "Big Blind";
            public const string Utg = "UTG";
            public const string Utg1 = "UTG+1";
            //Mid
            public const string MidPosition1 = "Mid Position1";
            public const string MidPosition2 = "Mid Position2";
            public const string MidPosition3 = "Mid Position3";
            //Late
            public const string Hiack = "Hijack";
            public const string Cutoff = "Cutoff";
            public const string Dealer = "Dealer";
        }

        public class HandActions
        {
            public const string Ante = "Ante";
            public const string SmallBlind = "Small Blind";
            public const string BigBlind = "Big Blind";
            public const string Raise = "Raise";
            public const string River = "River";
            public const string Fold = "Fold";
            public const string Call = "Call";
            public const string Check = "Check";
            public const string Bet = "Bet";
            public const string Collected = "Collected";
            public const string ShowHand = "Show Hand";
        }

        private static Dictionary<string, short> InitSteps()
        {
            var steps = new Dictionary<string, short>();

            steps[HandSteps.AnteBlinds] = 1;
            steps[HandSteps.PreFlop] = 2;
            steps[HandSteps.Flop] = 3;
            steps[HandSteps.Turn] = 4;
            steps[HandSteps.River] = 5;
            steps[HandSteps.Showdown] = 6;
            return steps;
        }


        private static Dictionary<string, Dim_ActionDal> InitActions()
        {
            var actionType = new Dictionary<string, Dim_ActionDal>();

            actionType["posts ante"] = new Dim_ActionDal(1, HandActions.Ante);
            actionType["posts small blind"] = new Dim_ActionDal(2, HandActions.SmallBlind);
            actionType["posts big blind"] = new Dim_ActionDal(3, HandActions.BigBlind);
            actionType["raises"] = new Dim_ActionDal(4, HandActions.Raise);
            actionType["folds"] = new Dim_ActionDal(5, HandActions.Fold);
            actionType["calls"] = new Dim_ActionDal(6, HandActions.Call);
            actionType["checks"] = new Dim_ActionDal(7, HandActions.Check);
            actionType["bets"] = new Dim_ActionDal(8, HandActions.Bet);
            actionType["collected"] = new Dim_ActionDal(9, HandActions.Collected);
            actionType["shows"] = new Dim_ActionDal(10, HandActions.ShowHand);
            return actionType;
        }

        public enum FilterType
        {
            CurrentSession=0,
            GameType =1,
            PlayerPosition =2,
            LastnHands=3,
            BuyIn=4,
            OnlyRealMoney=5,
            DateTime=6

        }
    }
}