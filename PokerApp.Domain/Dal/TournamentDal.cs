using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PokerApp.Domain.Dal
{
    [Table("Tournament")]
    public class TournamentDal
    {
        [Key]
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
    }
}
