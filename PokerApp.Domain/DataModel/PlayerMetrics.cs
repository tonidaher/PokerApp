using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public class PlayerMetrics
    {
        public string PlayerName { get; set; }
        public int SeatNumber { get; set; }
        public string CurrentTournamentId { get; set; }
        public string CurrentTournamentName { get; set; }
        public int HandsCount { get; set; }
        public int PreFlopFold { get; set; }
        public int PreFlopRaise { get; set; }
        public int PreFlopCheck { get; set; }
        public int PreFlopCall { get; set; }

        public int FlopFold { get; set; }
        public int FlopCheck { get; set; }
        public int FlopCall { get; set; }
        public int FlopBet { get; set; }
        public int FlopRaise { get; set; }

        public int TurnFold { get; set; }
        public int TurnCheck { get; set; }
        public int TurnCall { get; set; }
        public int TurnBet { get; set; }
        public int TurnRaise { get; set; }

        public int RiverFold { get; set; }
        public int RiverCheck { get; set; }
        public int RiverCall { get; set; }
        public int RiverBet { get; set; }
        public int RiverRaise { get; set; }

        public int Cbet { get; set; }
        public int Vpip { get; set; }
        public double Af { get; set; }

        public int InF { get; set; }
        public int InT { get; set; }
        public int InR { get; set; }
        public int InS { get; set; }

    }
}
