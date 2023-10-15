
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PokerApp.Domain.Dal
{
    [Table("Dim_Action")]
    public class Dim_ActionDal
    {
        [Key]
        public short ActionTypeId { get; set; }
        public string ActionName { get; set; }

        public Dim_ActionDal(short actionTypeId, string actionName)
        {
            ActionTypeId = actionTypeId;
            ActionName = actionName;
        }
    }
}
