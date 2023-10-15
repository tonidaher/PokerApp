using PokerApp.Domain;
using PokerApp.Domain.Common;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokerApp.Data.SaveToDatabase
{
    public class MemoryHandReader : IMemoryHandReader
    {
        public void ReadPlayerSeated(string line, Hand hand, out Player player, out PlayerHand playerHand)
        {
            Regex regex = new Regex(Constants.FilePattern.PlayerSeatedPattern);
            var match = regex.Match(line);
            player = new Player(match.Groups["PlayerName"].Value);
            playerHand = new PlayerHand();
            playerHand.PlayerName = player.PlayerName;
            playerHand.HandId = hand.HandId;
            playerHand.SeatNumber = int.Parse(match.Groups["SeatNumber"].Value);
            playerHand.InitialStack = double.Parse(match.Groups["InitialStack"].Value, CultureInfo.InvariantCulture);
            playerHand.EndStack = playerHand.InitialStack;
        }

        public bool ReadTournament(string fileName, out Tournament tournament)
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
                tournament = new Tournament() { Filename = f.Name, TournamentType = tournamentType, TournamentId = tournamentId, TournamentName = name, IsRealMoney = isReal, Date = date };
                return true;
            }
            return false;
        }

        public void UpdateHandHeader(string line, out Hand hand, Tournament tournament, int newLines)
        {
            hand = new Hand();
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
                hand.HandLevel = int.Parse(match.Groups["HandLevel"].Value);
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

        private void ReadBlinds(Hand hand, Match match)
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

        public void ReadHandDealt(string line, Dictionary<string, PlayerHand> playersInHand)
        {
            var pattern = Constants.FilePattern.PlayerDealtPattern.Replace(Constants.FilePattern.PlayersVariable, string.Join("|", playersInHand.Keys));
            Regex regex = new Regex(pattern);
            var match = regex.Match(line);
            var playerName= match.Groups["PlayerName"].Value;
            var cards = match.Groups["Cards"].Value;
            UpdateCards(playersInHand[playerName], cards);
        }
        public DealtPotAmount ReadHandAction(string line, Dictionary<string, PlayerHand> playersInHand, Hand hand, double potAmount, out HandAction handAction, int actionOrder, string step)
        {
            handAction = new HandAction();
            var pattern = Constants.FilePattern.HandActionPattern.Replace(Constants.FilePattern.PlayersVariable, string.Join("|", playersInHand.Keys));
            Regex regex = new Regex(pattern);
            var match = regex.Match(line);
            handAction.PlayerName = match.Groups["PlayerName"].Value;
            var action = Constants.Actions[match.Groups["ActionName"].Value];
            handAction.ActionTypeId = action.ActionTypeId;
            handAction.ActionOrder = actionOrder;
            handAction.HandId = hand.HandId;
            var raiseAmountDescription = match.Groups["PotAmount"].Value;
            var raiseAmount =  raiseAmountDescription.IsNullOrBlank() ? 0 : double.Parse(raiseAmountDescription, CultureInfo.InvariantCulture);
            var amountDescription = match.Groups["Amount"].Value;
            handAction.Amount = amountDescription.IsNullOrBlank() ? 0 : double.Parse(amountDescription, CultureInfo.InvariantCulture);

            handAction.IsAllIn = match.Groups["IsAllIn"].Value.Length > 0;
            handAction.StepId = Constants.Steps[step];
            bool isCollected = action.ActionName.Equals(Constants.HandActions.Collected);

            var newPotAmount = isCollected ? potAmount : potAmount + handAction.Amount;
            if (isCollected) { playersInHand[handAction.PlayerName].Collected = true; playersInHand[handAction.PlayerName].EndStack += handAction.Amount; }
            handAction.PotAmount = newPotAmount;
            UpdatePlayerInHand(playersInHand[handAction.PlayerName], step, action.ActionName, match.Groups["Cards"].Value, handAction.Amount, raiseAmount);
            return new DealtPotAmount() { IsDealt = isCollected, PotAmount = newPotAmount };
        }

        private void UpdatePlayerInHand(PlayerHand join_PlayerHand, string step, string action, string cards, double amount, double raiseAmount)
        {
            switch (step)
            {
                case Constants.HandSteps.AnteBlinds:
                    UpdateBlinds(join_PlayerHand, action,amount);
                    break;
                case Constants.HandSteps.PreFlop:
                    UpdatePreflop(join_PlayerHand, action, amount, raiseAmount);
                    break;
                case Constants.HandSteps.Flop:
                    UpdateFlop(join_PlayerHand, action, amount, raiseAmount);
                    break;
                case Constants.HandSteps.Turn:
                    UpdateTurn(join_PlayerHand, action, amount, raiseAmount);
                    break;
                case Constants.HandSteps.River:
                    UpdateRiver(join_PlayerHand, action, amount, raiseAmount);
                    break;
                case Constants.HandSteps.Showdown:
                    UpdateShowdown(join_PlayerHand, action, cards, amount);

                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }
        }

        private void UpdateShowdown(PlayerHand join_PlayerHand, string action, string cards,double amount)
        {
            join_PlayerHand.SeeShowdown = true;
            if (action == Constants.HandActions.Collected)
            {
                join_PlayerHand.Collected = true;

            }
            else if (action == Constants.HandActions.ShowHand)
            {
                UpdateCards(join_PlayerHand, cards);
            }
        }

        private void UpdateCards(PlayerHand join_PlayerHand, string cards)
        {
           var splitedCards = cards.Split(' ');
            var count = splitedCards.Count();
            if (count > 0) join_PlayerHand.Card1 = splitedCards[0];
            if (count > 1) join_PlayerHand.Card2 = splitedCards[1];
            if (count > 2) join_PlayerHand.Card3 = splitedCards[2];
            if (count > 3) join_PlayerHand.Card4 = splitedCards[3];
        }

        private void UpdateBlinds(PlayerHand join_PlayerHand, string action, double amount)
        {
            join_PlayerHand.EndStack -= amount;

            switch (action)
            {
                case Constants.HandActions.SmallBlind:
                    join_PlayerHand.IsSmallBlind = true;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.BigBlind:
                    join_PlayerHand.IsBigBlind = true;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
            }
        }

        private void UpdatePreflop(PlayerHand join_PlayerHand, string action, double amount, double raiseAmount)
        {
            switch (action)
            {

                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldPreflop = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallPreflop = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckPreflop = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetPreflop = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaisePreflop = true;
                    join_PlayerHand.EndStack -=  (raiseAmount - join_PlayerHand.PostedCurrentStage);
                    join_PlayerHand.PostedCurrentStage = raiseAmount;
                    break;
            }
        }

        private void UpdateFlop(PlayerHand join_PlayerHand, string action,double amount, double raiseAmount)
        {
            join_PlayerHand.Seeflop = true;
            switch (action)
            {
                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldFlop = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallFlop = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckFlop = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetFlop = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseFlop = true;
                    join_PlayerHand.EndStack -= (raiseAmount - join_PlayerHand.PostedCurrentStage);
                    join_PlayerHand.PostedCurrentStage = raiseAmount;
                    break;
            }
        }

        private void UpdateTurn(PlayerHand join_PlayerHand, string action, double amount, double raiseAmount)
        {
            join_PlayerHand.SeeTurn = true;
            switch (action)
            {
                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldTurn = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallTurn = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckTurn = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetTurn = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseTurn = true;
                    join_PlayerHand.EndStack -= (raiseAmount - join_PlayerHand.PostedCurrentStage);
                    join_PlayerHand.PostedCurrentStage = raiseAmount;
                    break;
            }
        }

        private void UpdateRiver(PlayerHand join_PlayerHand, string action, double amount, double raiseAmount)
        {
            join_PlayerHand.SeeRiver = true;
            switch (action)
            {

                case Constants.HandActions.Fold:
                    join_PlayerHand.FoldRiver = true;
                    break;
                case Constants.HandActions.Call:
                    join_PlayerHand.CallRiver = true;
                    join_PlayerHand.EndStack -= amount;
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Check:
                    join_PlayerHand.CheckRiver = true;
                    break;
                case Constants.HandActions.Bet:
                    join_PlayerHand.BetRiver = true;
                    join_PlayerHand.EndStack -= amount; 
                    join_PlayerHand.PostedCurrentStage += amount;
                    break;
                case Constants.HandActions.Raise:
                    join_PlayerHand.RaiseRiver = true;
                    join_PlayerHand.EndStack -= (raiseAmount - join_PlayerHand.PostedCurrentStage);
                    join_PlayerHand.PostedCurrentStage = raiseAmount;
                    break;
            }
        }


    }
}
