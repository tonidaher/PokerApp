using HoldemHand;
using data = PokerApp.Domain.DataModel;
using System;

namespace PokerApp.Domain.HandEvaluation
{
    public class HoldemHandEvaluator : IHoldemHandEvaluator
    {
        public data.HandEvaluationResult EvaluateHand(string cards, string board, int playersCount)
        {
            double[] player;
            double[] opponent;
            try
            {
                Hand.HandWinOdds(Hand.ParseHand(cards), Hand.ParseHand(board), out player, out opponent, playersCount, 2.0);
            }
            catch { return new data.HandEvaluationResult(); }
            var handEvaluationResult = new data.HandEvaluationResult();
            for (int i = 0; i < 9; i++)
            {
                switch ((Hand.HandTypes)i)
                {
                    case Hand.HandTypes.HighCard:
                        handEvaluationResult.Player.HighCard = player[i];
                        handEvaluationResult.Opponent.HighCard = opponent[i];
                        break;
                    case Hand.HandTypes.Pair:
                        handEvaluationResult.Player.Pair = player[i];
                        handEvaluationResult.Opponent.Pair = opponent[i];
                        break;
                    case Hand.HandTypes.TwoPair:
                        handEvaluationResult.Player.TwoPair = player[i];
                        handEvaluationResult.Opponent.TwoPair = opponent[i];
                        break;
                    case Hand.HandTypes.Trips:
                        handEvaluationResult.Player.ThreeOfAKind = player[i];
                        handEvaluationResult.Opponent.ThreeOfAKind = opponent[i];
                        break;
                    case Hand.HandTypes.Straight:
                        handEvaluationResult.Player.Straight = player[i];
                        handEvaluationResult.Opponent.Straight = opponent[i];
                        break;
                    case Hand.HandTypes.Flush:
                        handEvaluationResult.Player.Flush = player[i];
                        handEvaluationResult.Opponent.Flush = opponent[i];
                        break;
                    case Hand.HandTypes.FullHouse:
                        handEvaluationResult.Player.FullHouse = player[i];
                        handEvaluationResult.Opponent.FullHouse = opponent[i];
                        break;
                    case Hand.HandTypes.FourOfAKind:
                        handEvaluationResult.Player.FourOfAKind = player[i];
                        handEvaluationResult.Opponent.FourOfAKind = opponent[i];
                        break;
                    case Hand.HandTypes.StraightFlush:
                        handEvaluationResult.Player.StraightFlush = player[i];
                        handEvaluationResult.Opponent.StraightFlush = opponent[i];
                        break;
                }
                handEvaluationResult.Player.WinPct += player[i];
                handEvaluationResult.Opponent.WinPct += opponent[i];

            }
            return FormatPercentage(handEvaluationResult);

        }

        private data.HandEvaluationResult FormatPercentage(data.HandEvaluationResult handEvaluationResult)
        {
            handEvaluationResult.Player.HighCard = Math.Round(handEvaluationResult.Player.HighCard *100.0, 2) ;
            handEvaluationResult.Player.Pair = Math.Round(handEvaluationResult.Player.Pair *100.0, 2);
            handEvaluationResult.Player.TwoPair = Math.Round(handEvaluationResult.Player.TwoPair * 100.0, 2);
            handEvaluationResult.Player.ThreeOfAKind = Math.Round(handEvaluationResult.Player.ThreeOfAKind * 100.0, 2);
            handEvaluationResult.Player.Straight = Math.Round(handEvaluationResult.Player.Straight * 100.0, 2);
            handEvaluationResult.Player.Flush = Math.Round(handEvaluationResult.Player.Flush * 100.0, 2);
            handEvaluationResult.Player.FullHouse = Math.Round(handEvaluationResult.Player.FullHouse * 100.0, 2);
            handEvaluationResult.Player.FourOfAKind = Math.Round(handEvaluationResult.Player.FourOfAKind * 100.0, 2);
            handEvaluationResult.Player.StraightFlush = Math.Round(handEvaluationResult.Player.StraightFlush * 100.0, 2);
            handEvaluationResult.Player.WinPct = Math.Round(handEvaluationResult.Player.WinPct * 100.0, 2);

            handEvaluationResult.Opponent.HighCard = Math.Round(handEvaluationResult.Opponent.HighCard * 100.0, 2);
            handEvaluationResult.Opponent.Pair = Math.Round(handEvaluationResult.Opponent.Pair * 100.0, 2);
            handEvaluationResult.Opponent.TwoPair = Math.Round(handEvaluationResult.Opponent.TwoPair * 100.0, 2);
            handEvaluationResult.Opponent.ThreeOfAKind = Math.Round(handEvaluationResult.Opponent.ThreeOfAKind * 100.0, 2);
            handEvaluationResult.Opponent.Straight = Math.Round(handEvaluationResult.Opponent.Straight * 100.0, 2);
            handEvaluationResult.Opponent.Flush = Math.Round(handEvaluationResult.Opponent.Flush * 100.0, 2);
            handEvaluationResult.Opponent.FullHouse = Math.Round(handEvaluationResult.Opponent.FullHouse * 100.0, 2);
            handEvaluationResult.Opponent.FourOfAKind = Math.Round(handEvaluationResult.Opponent.FourOfAKind * 100.0, 2);
            handEvaluationResult.Opponent.StraightFlush = Math.Round(handEvaluationResult.Opponent.StraightFlush * 100.0, 2);
            handEvaluationResult.Opponent.WinPct = Math.Round(handEvaluationResult.Opponent.WinPct * 100.0, 2);

            return handEvaluationResult;
        }

        public double FormatPercent(double v)
        {
            return Math.Round(v * 100.0, 2);
        }
    }

}
