using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerApp.Domain.Dal
{
    [Table("Join_PlayerHand")]
    public class Join_PlayerHandDal
    {

        [Key]
        public string PlayerName { get; set; }
        [Key]
        public string HandId { get; set; }

        public int SeatNumber { get; set; }

        public double InitialStack { get; set; }

        public string Card1 { get; set; }
        public string Card2 { get; set; }

        public string Card3 { get; set; }
        public string Card4 { get; set; }

        public bool IsBigBlind { get; set; }

        public bool IsSmallBlind { get; set; }
        public double EndStack { get; set; }

        public bool Seeflop { get; set; }
        public bool SeeTurn { get; set; }
        public bool SeeRiver { get; set; }
        public bool SeeShowdown { get; set; }

        public bool FoldPreflop { get; set; }
        public bool FoldFlop { get; set; }
        public bool FoldTurn { get; set; }
        public bool FoldRiver { get; set; }
        public bool CallPreflop { get; set; }
        public bool CallFlop { get; set; }
        public bool CallTurn { get; set; }
        public bool CallRiver { get; set; }

        public bool BetPreflop { get; set; }
        public bool BetFlop { get; set; }
        public bool BetTurn { get; set; }
        public bool BetRiver { get; set; }

        public bool CheckPreflop { get; set; }
        public bool CheckFlop { get; set; }
        public bool CheckTurn { get; set; }
        public bool CheckRiver { get; set; }

        public bool RaisePreflop { get; set; }
        public bool RaiseFlop { get; set; }
        public bool RaiseTurn { get; set; }
        public bool RaiseRiver { get; set; }
        public bool Collected { get; set; }

        [ForeignKey("PlayerName")]
        public PlayerDal Player { get; set; }

        [ForeignKey("HandId")]
        public HandDal Hand { get; set; }

    }
}
