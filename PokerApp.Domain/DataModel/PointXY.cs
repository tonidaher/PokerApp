using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
   public struct PointXY
    {

        public PointXY(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
