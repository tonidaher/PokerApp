using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PokerApp.Domain.Dal
{
    [Table("Hand")]
    public class HandDal
    {
        [Key]
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

        public virtual TournamentDal Tournament { get; set; }
    }
}
