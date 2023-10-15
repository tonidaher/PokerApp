using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
    public class HandCardMetrics
    {

        public string Card { get; set; }
        public int Count { get; set; }

        public int VpipCount { get; set; }
        public int WinCount { get; set; }

        public int WinPercent { get; set; }
        public int WinAndVpipPercent { get; set; }
        public int RoiBBAmount { get; set; }

    }
}
