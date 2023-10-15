using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public class PlayerHandEvaluationResult
    {
        public double HighCard { get; set; }
        public double Pair { get; set; }
        public double TwoPair { get; set; }
        public double ThreeOfAKind { get; set; }
        public double Straight { get; set; }
        public double Flush { get; set; }
        public double FullHouse { get; set; }
        public double FourOfAKind { get; set; }
        public double StraightFlush { get; set; }
    
        public double WinPct { get; set; }
    }
}
