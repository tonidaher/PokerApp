using System;
using System.Runtime.InteropServices;
using WindowScrape.Types;
using System.Linq;
using System.Drawing;
using System.Xml.Serialization;
using System.Drawing.Imaging;
using System.Threading;

namespace PokerApp.Domain
{
    public class WindowPositionGetter : IWindowPositionGetter
    {
        public HwndObject GetWindowHandler(string tournamentId, string tournamentName)
        {
           return HwndObject.GetWindows().FirstOrDefault(x => x.Title.Contains(tournamentId) && x.Title.Contains(tournamentName));
        }
        public Rect GetPosition(string tournamentId, string tournamentName)
        {
            var window = GetWindowHandler(tournamentId, tournamentName);
            if (window == null)
            {
                return new Rect() { Bottom = 500, Right = 500 };
            }
            return new Rect() { Left = window.Location.X, Right = window.Location.X + window.Size.Width, Top = window.Location.Y, Bottom = window.Location.Y + window.Size.Height };
        }
    }

    public static class Win32
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        [DllImport("User32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, ref Rect lpRect);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }
}