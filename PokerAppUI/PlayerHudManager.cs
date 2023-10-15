using PokerApp.Domain;
using System.Collections.Generic;
using System.Windows ;
using System.Linq;
using System.Windows.Threading;
using Rect = PokerApp.Domain.Rect;
using PokerApp.Domain.DataModel;
using PokerAppUI.Struct;
using Serilog;

namespace PokerAppUI
{
    public class PlayerHudManager
    {
        private Dictionary<string, PlayerHud> hudByPlayer;
        private Dictionary<TournamentStruct, Rect> _windowPositions;
        private Dictionary<TournamentStruct, HandEvaluationWindow> _handEvaluationWindows;
        public static IPlayerMetricCalculator Calculator { get; set; }
        public static ICardMetricCalculator CardMetricCalculator { get; set; }
        private Dispatcher _dispatcher;
        private IWindowPositionGetter _windowPositionGetter;
        private IPlayerPositionGetter _playerPositionGetter;
        private string _accountName;
        private ILogger _logger;
        public static Dictionary<string,MetricThreshold> MetricThresholds { get; private set; }

        public PlayerHudManager(IPlayerMetricCalculator calculator,ICardMetricCalculator cardMetricCalculator, IWindowPositionGetter windowPositionGetter, IPlayerPositionGetter playerPositionGetter, Dispatcher dispatcher, string accountName,ILogger logger)
        {
            _logger = logger;
            Calculator = calculator;
            CardMetricCalculator = cardMetricCalculator;
            _windowPositionGetter = windowPositionGetter;
            _playerPositionGetter = playerPositionGetter;
            _windowPositions = new Dictionary<TournamentStruct, Rect>();
            _handEvaluationWindows = new Dictionary<TournamentStruct, HandEvaluationWindow>();
            hudByPlayer = new Dictionary<string, PlayerHud>();
            _dispatcher = dispatcher;
            _accountName = accountName;
            MetricThresholds = Calculator.GetThresholds().ToDictionary(x => x.MetricName);
            Update();
        }

        public void Update()
        {
                 
            var metrics = Calculator.GetCurrentPlayers(); 
            IEnumerable<TournamentStruct> currentTables = metrics.Select(x => new TournamentStruct() { TournamentName = x.CurrentTournamentName, TournamentId = x.CurrentTournamentId }).Distinct();
             CalculateWindowPositions(currentTables);
             
            //Remove old players
            RemoveOldHud(metrics.Select(x => x.PlayerName));

            foreach (var player in metrics)
            {
                //Update existing players
                if (hudByPlayer.ContainsKey(player.PlayerName))
                {
                    hudByPlayer[player.PlayerName].Update(player);
                }
                else
                {
                    //Create new Players
                    _dispatcher.Invoke(() =>
                    {
                        Point pos = CalculateHudPosition(player, metrics);
                        PlayerHud hud = new PlayerHud(player,pos);
                        hudByPlayer.Add(player.PlayerName, hud);
                    });
                }
            }
        }

        private Point CalculateHudPosition(PlayerMetrics player, IList<PlayerMetrics> metrics)
        {
            _logger.Information($"Calculating  Position:name={player.PlayerName} tournament = {player.CurrentTournamentName}");
            var windowSize = _windowPositions[new TournamentStruct() { TournamentId = player.CurrentTournamentId, TournamentName = player.CurrentTournamentName }];
            _logger.Information($"Calculating  Position:  tournament = {player.CurrentTournamentName} window=left={windowSize.Left},right={windowSize.Right},top={windowSize.Top},bottom={windowSize.Bottom}");
            var defaultPoint = new Point((windowSize.Right - windowSize.Left) / 2 + 20, (windowSize.Bottom - 60));
            var maxPlayers = metrics.Where(x => x.CurrentTournamentId == player.CurrentTournamentId && x.CurrentTournamentName == player.CurrentTournamentName).Select(x => x.SeatNumber).Max();
            var hero = metrics.FirstOrDefault(x => x.PlayerName == _accountName);
            if (hero == null) { return defaultPoint; }
            var pointxy = _playerPositionGetter.GetPlayerPosition(windowSize, player.SeatNumber, hero.SeatNumber, maxPlayers);
            _logger.Information($"Calculated Player Position: name={player.PlayerName} X={pointxy.X} Y={pointxy.Y}");
            return new Point(pointxy.X, pointxy.Y);
        }   


        private void CalculateWindowPositions(IEnumerable<TournamentStruct> currentTables)
        {
            var toRemove = _windowPositions.Keys.ToList().Except(currentTables);
            foreach (var key in toRemove)
            {
                _windowPositions.Remove(key);
                _dispatcher.Invoke(() =>
                {
                    _handEvaluationWindows[key].Dispose();
                });
                _handEvaluationWindows.Remove(key);
            }

            foreach (var tournament in currentTables)
            {
                if (!_windowPositions.ContainsKey(tournament))
                {
                    var position = _windowPositionGetter.GetPosition(tournament.TournamentId, tournament.TournamentName);
                    _dispatcher.Invoke(() =>
                    {
                        var handEvaluationWidow = new HandEvaluationWindow(tournament, position,_accountName);
                        _handEvaluationWindows.Add(tournament, handEvaluationWidow);
                    });
                        _windowPositions.Add(tournament, position);
                   
                }

            }

        }

        private void RemoveOldHud(IEnumerable<string> newPlayers)
        {
            var hudsToRemove =  hudByPlayer.Keys.Except(newPlayers).ToList();
            foreach(var hud in hudsToRemove)
            {
                DisposeHud(hud);  
            }
        }

        private void DisposeHud(string hud)
        {
            hudByPlayer[hud].Dispose();
            hudByPlayer.Remove(hud);
        }

        public void Dispose()
        {
            foreach(var hud in hudByPlayer.Values)
            {
                hud.Dispose();
            }
            foreach(var window in _handEvaluationWindows.Values)
            {
                window.Dispose();
            }
        }

        public void OnFilterUpdate()
        {
            foreach(var handWindow in _handEvaluationWindows)
            {
                handWindow.Value.UpdateCardsMetrics();
            }
            
        }

    }
}
