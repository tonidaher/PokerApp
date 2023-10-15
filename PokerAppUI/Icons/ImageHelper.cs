using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace PokerAppUI.Icons
{
   public static class ImageHelper

    {
        public static BitmapImage LoadImage(string filename)
        {
            if (filename == null) return null;
            filename = filename.Replace("10", "T");
            return new BitmapImage(new Uri("pack://application:,,,/Icons/Deck_Icons/" + filename + ".JPG"));
        }
    }
}
