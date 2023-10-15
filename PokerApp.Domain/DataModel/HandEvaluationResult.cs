using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
    public class HandEvaluationResult
    {
        public HandEvaluationResult()
        {
            Player = new PlayerHandEvaluationResult();
            Opponent = new PlayerHandEvaluationResult();
        }
        public PlayerHandEvaluationResult Player { get; set; }
        public PlayerHandEvaluationResult Opponent { get; set; }


    }
}
