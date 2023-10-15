using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PokerApp.Domain.Dal
{
    [Table("Dim_Step")]
    public class Dim_StepDal
    {

        [Key]
        public short StepId { get; set; }
        public string StepName { get; set; }

        public Dim_StepDal(short stepId, string stepName)
        {
            StepId = stepId;
            StepName = stepName;
        }
    }
}
