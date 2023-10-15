using PokerApp.Domain;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System.Collections.Generic;

namespace PokerApp.Data
{
    public class HandReader : IHandReader
    {


        public void ReadPlayerSeated(string line, HandDal hand, out PlayerDal player, out Join_PlayerHandDal playerHand)
        {
            Regex regex = new Regex(Constants.FilePattern.PlayerSeatedPattern);
            var match = regex.Match(line);
            player= new PlayerDal(match.Groups["PlayerName"].Value);
            playerHand = new Join_PlayerHandDal();
            playerHand.PlayerName = player.PlayerName;
            playerHand.HandId = hand.HandId;
            playerHand.SeatNumber = int.Parse(match.Groups["SeatNumber"].Value);
            playerHand.InitialStack = double.Parse(match.Groups["InitialStack"].Value, CultureInfo.InvariantCulture);
        }

        public bool ReadTournament(string fileName, out TournamentDal tournament)
        {
            tournament = null;
            if (fileName.EndsWith("_summary.txt"))
            {
                return false;
            }
            Regex regex = new Regex(Constants.FilePattern.FilaNamePattern);
            if (regex.IsMatch(fileName))
            {
                FileInfo f = new FileInfo(fileName);
               
                var match = regex.Match(fileName);
                var date = DateTime.ParseExact(match.Groups["date"].Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                var name = match.Groups["name"].Value;
                var tournamentId = 0;
                if (name.Contains('('))
                {
                    var splitName = name.Split('(', ')');
                    tournamentId = int.Parse(splitName[1]);
                    name = splitName[0];
                }
                var isReal = match.Groups["isreal"].Value == "real";
                var tournamentType = match.Groups["type"].Value.Replace('_', ' ');
                tournament = new TournamentDal() { Filename = f.Name, TournamentType = tournamentType, TournamentId = tournamentId, TournamentName = name, IsRealMoney = isReal, Date = date };
                return true;
            }
            return false;
        }

        public void UpdateHandHeader(string line, out HandDal hand, TournamentDal tournament,int newLines)
        {
            hand = new HandDal();
            hand.TournamentFileName = tournament.Filename;
            if (tournament.TournamentId != 0)
            {
                Regex regex = new Regex(Constants.FilePattern.HandBeginTournamentPattern);
                var match = regex.Match(line);
                if (tournament.BuyIn.IsNullOrBlank()) tournament.BuyIn = match.Groups["BuyIn"].Value;
                tournament.PokerSite = match.Groups["PokerSite"].Value;
                tournament.TournamentIndicator = Constants.TournamentIndicator.Tournament;
               
                hand.HandId = match.Groups["HandId"].Value;
                hand.HandName = hand.HandId;
                hand.HandLevel =int.Parse(match.Groups["HandLevel"].Value);
                hand.TimeStamp = DateTime.ParseExact(match.Groups["TimeStamp"].Value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                tournament.Lines += newLines;
                hand.Line = tournament.Lines;

                ReadBlinds(hand, match);
            }
            else
            {
                Regex regex = new Regex(Constants.FilePattern.HandBeginCashGamePattern);
                var match = regex.Match(line);
                tournament.PokerSite = match.Groups["PokerSite"].Value;
                tournament.TournamentIndicator = Constants.TournamentIndicator.CashGame;
                hand.HandId = match.Groups["HandId"].Value;
                hand.HandName = hand.HandId;
                hand.TimeStamp = DateTime.ParseExact(match.Groups["TimeStamp"].Value, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                ReadBlinds(hand, match);
            }
        }

        private void ReadBlinds(HandDal hand, Match match)
        {
            var blind3 = match.Groups["Blind3"].Value;
            if (!blind3.Equals(string.Empty))
            {
                hand.Ante = double.Parse(match.Groups["Blind1"].Value, CultureInfo.InvariantCulture);
                hand.SmallBlind = double.Parse(match.Groups["Blind2"].Value, CultureInfo.InvariantCulture);
                hand.BigBlind = double.Parse(match.Groups["Blind3"].Value, CultureInfo.InvariantCulture);
          
            }
            else
            {
                hand.SmallBlind = double.Parse(match.Groups["Blind1"].Value, CultureInfo.InvariantCulture);
                hand.BigBlind = double.Parse(match.Groups["Blind2"].Value, CultureInfo.InvariantCulture);
            }
        }

        public DealtPotAmount ReadHandAction(string line, Dictionary<string, Join_PlayerHandDal> playersInHand, string handId, double potAmount, out Join_HandActionDal handAction, int actionOrder, string step)
        {
            handAction = new Join_HandActionDal();
            var pattern = Constants.FilePattern.HandActionPattern.Replace(Constants.FilePattern.PlayersVariable, string.Join("|", playersInHand.Keys));
            Regex regex = new Regex(pattern);
            var match = regex.Match(line);
            handAction.PlayerName = match.Groups["PlayerName"].Value;
            var action  = Constants.Actions[match.Groups["ActionName"].Value];
            handAction.ActionTypeId = action.ActionTypeId;
            handAction.ActionOrder = actionOrder;
            handAction.HandId = handId;
            var amountDescription = match.Groups["Amount"].Value;
            handAction.Amount = amountDescription.IsNullOrBlank() ? 0 : double.Parse(amountDescription, CultureInfo.InvariantCulture);

            handAction.IsAllIn  = match.Groups["IsAllIn"].Value.Length>0;
            handAction.StepId = Constants.Steps[step];
            bool isDealt = action.ActionName.Equals(Constants.HandActions.Collected);

            var newPotAmount =isDealt? potAmount : potAmount + handAction.Amount;
            handAction.PotAmount = newPotAmount;
            UpdatePlayerInHand(playersInHand[handAction.PlayerName], step,action.ActionName) ;
            return new DealtPotAmount() { IsDealt = isDealt, PotAmount = newPotAmount };
        }

        private void UpdatePlayerInHand(Join_PlayerHandDal join_PlayerHand, string step, string action)
        {
           switch(step)
            {
                case Constants.HandSteps.AnteBlinds:
                    UpdateBlinds(join_PlayerHand, action);
                    break;
                case Constants.HandSteps.PreFlop:
                    UpdatePreflop(join_PlayerHand, action);
                    break;
                case Constants.HandSteps.Flop:
                    UpdateFlop(join_PlayerHand, action);
                    break;
                case Constants.HandSteps.Turn:
                    UpdateTurn(join_PlayerHand, action);
                    break;
                case Constants.HandSteps.River:
                    UpdateRiver(join_PlayerHand, action);
                    break;
                case Constants.HandSteps.Showdown:
                    join_PlayerHand.SeeShowdown = true;
                    break;
            } 
        }

        private void UpdateBlinds(Join_PlayerHandDal join_PlayerHand, string action)
        {
            switch (action)
            {
                case Constants.HandActions.SmallBlind:
                    join_PlayerHand.IsSmallBlind = true;
                    break;
                case Constants.HandActions.BigBlind:
                    join_PlayerHand.IsBigBlind = true;
                    break;
            }
        }

        private void UpdatePreflop(Join_PlayerHandDal join_PlayerHand, string action)
        {
            switch(action)
            {
                case Constants.HandActions.SmallBlind:
                    join_PlayerHand.IsSmallBlind = true;
                    break;
                case Constants.HandActions.BigBlind:
                    join_PlayerHand.IsBigBlind = true;
                    break;
                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldPreflop = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallPreflop = true;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckPreflop = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetPreflop = true;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaisePreflop = true;
                    break;
            }
        }

        private void UpdateFlop(Join_PlayerHandDal join_PlayerHand, string action)
        {
            join_PlayerHand.Seeflop = true;
            switch (action)
            {
                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldFlop = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallFlop = true;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckFlop = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetFlop = true;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseFlop = true;
                    break;
            }
        }

        private void UpdateTurn(Join_PlayerHandDal join_PlayerHand, string action)
        {
            join_PlayerHand.SeeTurn = true;
            switch (action)
            {
                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldTurn = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallTurn = true;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckTurn = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetTurn = true;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseTurn = true;
                    break;
            }
        }

        private void UpdateRiver(Join_PlayerHandDal join_PlayerHand, string action)
        {
            join_PlayerHand.SeeRiver = true;
            switch (action)
            {

                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldRiver = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallRiver = true;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckRiver = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetRiver = true;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseRiver = true;
                    break;
            }
        }
    }
}
