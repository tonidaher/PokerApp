using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PokerApp.Domain.Dal
{
    [Table("Player")]
    public class PlayerDal
    {

        public PlayerDal(string playerName)
        {
            PlayerName = playerName;
        }
        [Key]
        public string PlayerName { get; set; }

    }
}
