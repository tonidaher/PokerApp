using System;
using System.Collections.Generic;
using System.Text;
using WindowScrape.Types;

namespace PokerApp.Domain
{
    public interface IWindowPositionGetter
    {
        Rect GetPosition(string tournamentId, string tournamentName);
        HwndObject GetWindowHandler(string tournamentId, string tournamentName);
       
    }
}
