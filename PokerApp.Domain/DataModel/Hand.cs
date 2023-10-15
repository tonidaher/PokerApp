using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public class Hand : IEquatable<Hand>
    {
        public string HandId { get; set; }
        public string HandName { get; set; }
        public string TournamentFileName { get; set; }
        public int MaxPlayers { get; set; }
        public int DealerPosition { get; set; }
        public int HandLevel { get; set; }

        public double SmallBlind { get; set; }

        public double BigBlind { get; set; }

        public double Ante { get; set; }

        public DateTime TimeStamp { get; set; }

        public int Line { get; set; }

        public List<HandAction> HandActions { get; set; }

        public List<PlayerHand> PlayerHands { get; set; }

        public bool Equals(Hand other)
        {
            return HandId.Equals(other.HandId);
        }
    }
}
