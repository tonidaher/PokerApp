using HandHistoryParser;
using PokerApp.Data;
using PokerApp.Data.Logging;
using PokerApp.Data.SaveToDatabase;
using PokerApp.Domain;
using PokerApp.Domain.HandFilters;
using Serilog;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlayerHudManager _playerHudManager;
        public  HandFileReader _handFileReader;
        public static IHistoryHandContextProvider ContextProvider;
        public static string TempFolder = "temp";
        private readonly string _connectionString = @"Data Source=PC\SQLEXPRESS;Initial Catalog=Poker;Integrated Security=True";
        private readonly string _accountName = "metalingus87";
        private string _handsFolder;
        public static Dispatcher MainDispatcher;
        private ILogger _logger;
        private object locker = new object();
        public MainWindow()
        {
            _logger = new PokerLogger().GetLogger();
            ContextProvider = new HistoryHandContextProvider(_connectionString);
            InitializeComponent();
            CleanTemp();
            LoginWindow loginWindow = new LoginWindow();
            var loggedIn = loginWindow.ShowDialog();
            if (!loggedIn.HasValue || !loggedIn.Value) Close();
            _accountName = loginWindow.PlayerName;
            _handsFolder = loginWindow.HandsFolder;
            LaunchWatcher();
            LaunchSession();
 
        }

        private void CleanTemp()
        {
            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
                return;
            }
           var files = Directory.GetFiles("temp");
           
            foreach(var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        private void LaunchSession()
        {
            IHandFilterFactory filterFactory = new HandFilterFactory(ContextProvider);
            IPlayerMetricCalculator calculator = new PlayerMetricCalculator(ContextProvider,filterFactory);
            ICardMetricCalculator cardMetricCalculator = new CardMetricCalculator(ContextProvider, filterFactory);
            IWindowPositionGetter windowPositionGetter = new WindowPositionGetter();
            IPlayerPositionGetter playerPositionGetter = new PlayerPositionGetter();
            MainDispatcher = Dispatcher;
            _playerHudManager = new PlayerHudManager(calculator, cardMetricCalculator,windowPositionGetter, playerPositionGetter,Dispatcher,_accountName,_logger);
            hudOptionsGridView.DataContext = _playerHudManager;
        }

        private void LaunchWatcher()
        {
            HistoryHandContextProvider context = new HistoryHandContextProvider(_connectionString);
            IHandSaver saver = new DbHandSaver(context);
            IMemoryHandReader memoryReader = new MemoryHandReader();
            IMemoryHandSaver handSaver = new MemoryHandSaver(new MemoryHandContext());
            IHandReader reader = new HandReader();
            IHandLocationProvider handLocationProvider = new HandLocationProvider(@$"{_handsFolder}\accounts\{_accountName}\history");

            IFileWatcherService fileWatcherService = new FileWatcherService.FileWatcherService(handLocationProvider);
            fileWatcherService.NewHand += FileWatcherService_NewHand;
           
            _handFileReader = new HandFileReader(handLocationProvider, saver, handSaver, memoryReader,_logger);
            fileWatcherService.StartWatch();
        }

        private  void FileWatcherService_NewHand(string fileName)
        {
            lock (locker)
            {
                _logger.Information("New Hand Received");
                _handFileReader.ReadFileInMemory(fileName);
                _handFileReader.SaveToDatbase();
            }
            if(_playerHudManager!=null) _playerHudManager.Update();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
          if(_playerHudManager!=null) _playerHudManager.Dispose();
        }
    }
}
