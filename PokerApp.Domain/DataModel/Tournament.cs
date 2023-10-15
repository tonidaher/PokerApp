using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public class Tournament :IEquatable<Tournament>
    {
        public string Filename { get; set; }

        public int TournamentId { get; set; }

        public string TournamentName { get; set; }

        public string TournamentType { get; set; }
        public string TournamentIndicator { get; set; }

        public string BuyIn { get; set; }

        public bool IsRealMoney { get; set; }

        public string PokerSite { get; set; }

        public int Lines { get; set; }

        public int Bytes { get; set; }
        public DateTime Date { get; set; }

        List<Hand> Hands { get; set; }

        public bool Equals(Tournament other)
        {
            return Filename.Equals(other.Filename);
        }
    }
}
