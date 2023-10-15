using System;
using System.Drawing;
using System.IO;
using WindowScrape.Types;

namespace PokerApp.Domain
{
    public class ScreenshotTaker : IScreenshotTaker
    {
        public void DeleteCard(string fileName)
        {
            try
            {
                File.Delete(fileName);
            }
            catch { }
        }
        public Bitmap WindowScreen(HwndObject window)
        {

            var size = window.Size;
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            Graphics memoryGraphics = Graphics.FromImage(bitmap);
            IntPtr dc = memoryGraphics.GetHdc();
            Win32.PrintWindow(window.Hwnd, dc, 0);
            memoryGraphics.ReleaseHdc(dc);

            Rect rect = new Rect();
            if (Win32.GetWindowRect(window.Hwnd, ref rect))
            {
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                bitmap = new Bitmap(width, height);
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.CopyFromScreen(rect.Left, rect.Top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }
    }
}
