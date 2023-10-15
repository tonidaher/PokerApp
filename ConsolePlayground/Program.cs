using System;
using System.Text.RegularExpressions;
using DataAccess;
using HandHistoryParser;
using PokerApp.Data;
using PokerApp.Domain;
using PokerApp.Domain.Common;
using FileWatcherService;
using PokerApp.Data.SaveToDatabase;
using PokerApp.Domain.HandFilters;
using System.Drawing;
using System.Drawing.Imaging;
using PokerApp.Data.Logging;
using System.IO;
using System.Text;

namespace ConsolePlayground
{
    public class Program
    {
       public static HandFileReader _handFileReader;
        static void Main(string[] args)
        {
            var logger = new PokerLogger().GetLogger();
            logger.Information("Hello Logs");
            Console.WriteLine("Hello World!");
            WindowPositionGetter posGetter = new WindowPositionGetter();
            Rect r = posGetter.GetPosition("Exp", "VPN");
            
            //  HistoryHandContext context = new HistoryHandContext(@"Data Source=PC\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
            var contextProvider = new HistoryHandContextProvider(@"Data Source=PC\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True");
            var filterFactory = new HandFilterFactory(contextProvider);
            IHandSaver saver = new DbHandSaver(contextProvider);
            IHandReader reader = new HandReader();
            IHandLocationProvider handLocationProvider = new HandLocationProvider(@"C:\Users\Windows.000\Documents\Winamax Poker\accounts\metalingus87\history");

            //TO WATCH CURRENT SESSION
            //IFileWatcherService fileWatcherService = new FileWatcherService.FileWatcherService(handLocationProvider);
            //fileWatcherService.NewHand += FileWatcherService_NewHand;
            //fileWatcherService.StartWatch();
            // 


            //TO READ ALL HISTORY
            //IMemoryHandSaver handSaver = new MemoryHandSaver(new MemoryHandContext());
            //IMemoryHandReader memoryReader = new MemoryHandReader();
            //_handFileReader = new HandFileReader(handLocationProvider, saver, handSaver, memoryReader);
            //_handFileReader.ReadAllFilesInMemory();


            //_handFileReader = new HandFileReader(handLocationProvider,saver, new MemoryHandSaver(new MemoryHandContext()), new MemoryHandReader());
            //_handFileReader.ReadAllFilesInMemory();
            // DatabaseHandLoader loader = new DatabaseHandLoader(context);
            // var currentPlayers = loader.GetCurrentPlayers();


            //Calcualte Metrics

            // IPlayerMetricCalculator calculator = new PlayerMetricCalculator(contextProvider, filterFactory);
            //var myMetrics = calculator.GetPlayerMetrics("metalingus87");
            Console.ReadLine();
        }
        private static void FileWatcherService_NewHand(string fileName)
        {
            _handFileReader.ReadFileInMemory(fileName);
        }

        public static Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }



    }
}
