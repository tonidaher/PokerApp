using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerApp.Domain.Dal
{
    [Table("Join_HandAction")]
    public class Join_HandActionDal
    {
        [Key]
        [Column(Order = 1)]
        public string HandId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string PlayerName { get; set; }

        [Key]
        [Column(Order = 3)]
        public int ActionOrder { get; set; }

        public short ActionTypeId { get; set; }

        public short StepId { get; set; }

        public double Amount { get; set; }

        public double PotAmount { get; set; }
        public bool IsAllIn { get; set; }
        public Dim_ActionDal ActionType { get; set; }
        public Dim_StepDal Step { get; set; }

        public PlayerDal Player { get; set; }
        public HandDal Hand { get; set; }

    }
}
