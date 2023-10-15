using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerApp.Domain.Dal
{
    [Table("CurrentPlayers")]
    public class CurrentPlayersDal
    {
        [Key]
        public string PlayerName { get; set; }

        public int NbHands { get; set; }
        public string NbHandsDescription { get {return "("+NbHands+")"; } }

        public int TournamentId { get; set; }

        public string TournamentName { get; set; }

        public int SeatNumber { get; set; }
    }
}
