using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public class HandAction
    {
        public string HandId { get; set; }
        public string PlayerName { get; set; }

        public int ActionOrder { get; set; }

        public short ActionTypeId { get; set; }

        public short StepId { get; set; }

        public double Amount { get; set; }

        public double PotAmount { get; set; }
        public bool IsAllIn { get; set; }
    }
}
