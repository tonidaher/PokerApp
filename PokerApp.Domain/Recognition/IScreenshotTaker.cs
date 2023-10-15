using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using WindowScrape.Types;

namespace PokerApp.Domain
{
    public interface IScreenshotTaker
    {
        Bitmap WindowScreen(HwndObject handlerObject);

         void DeleteCard(string fileName);
    }
}
