using AutoMapper;
using PokerApp.Data.Logging;
using PokerApp.Domain;
using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HandHistoryParser
{
    public class HandFileReader
    {
        private string _handLocation;
        private IHandSaver _handSaver;

        private IMemoryHandSaver _memoryHandSaver;
        private IMemoryHandReader _memoryHandReader;
        private IMapper _mapper;
        private ILogger _logger;
   
        public HandFileReader(IHandLocationProvider handLocationProvider, IHandSaver dbHandSaver, IMemoryHandSaver handSaver, IMemoryHandReader handReader,ILogger logger)
        {
            _logger = logger;
            _handSaver = dbHandSaver;
            _memoryHandSaver = handSaver;
            _memoryHandReader = handReader;
            _handLocation = handLocationProvider.GetHandLocation();

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Tournament, TournamentDal>();
                cfg.CreateMap<Hand, HandDal>();
                cfg.CreateMap<Player, PlayerDal>();
                cfg.CreateMap<PlayerHand, Join_PlayerHandDal>();
                cfg.CreateMap<HandAction, Join_HandActionDal>();

            });

            _mapper = config.CreateMapper();
        }

        public void ReadAllFilesInMemory()
        {
            var files = Directory.EnumerateFiles(_handLocation);
            Parallel.ForEach(files, (file) =>
            {
                ReadFileInMemory(file);
            });

            SaveToDatbase();
        }

        public void SaveToDatbase()
        {            
            foreach (Tournament tournament in _memoryHandSaver.Context.Tournaments)
            {
                TournamentDal tournamentDal = _mapper.Map<TournamentDal>(tournament);
                _handSaver.SaveTournament(tournamentDal);
            }
            _handSaver.Commit();
            _logger.Information(_memoryHandSaver.Context.Tournaments.Count + " Tournament(s) saved");
            foreach (Hand hand in _memoryHandSaver.Context.Hands)
            {
                HandDal handDal = _mapper.Map<HandDal>(hand);
                _handSaver.SaveHand(handDal);
            }
            _handSaver.Commit();
            _logger.Information(_memoryHandSaver.Context.Hands.Count + " Hands(s) saved");
            foreach (Player player in _memoryHandSaver.Context.Players)
            {
                PlayerDal playerDal = _mapper.Map<PlayerDal>(player);
                _handSaver.SavePlayer(playerDal);
            }
            _handSaver.Commit();
            _logger.Information(_memoryHandSaver.Context.Players.Count + " Player(s) saved");
            foreach (PlayerHand hand in _memoryHandSaver.Context.PlayerHands)
            {
                Join_PlayerHandDal handDal = _mapper.Map<Join_PlayerHandDal>(hand);
                 _handSaver.SavePlayerHand(handDal);
            }
            _handSaver.Commit();

            _logger.Information(_memoryHandSaver.Context.PlayerHands.Count + " Player Hand(s) saved");
            foreach (HandAction hand in _memoryHandSaver.Context.HandActions)
            {
                Join_HandActionDal handDal = _mapper.Map<Join_HandActionDal>(hand);
                _handSaver.SaveHandAction(handDal);
            }
            _handSaver.Commit();
            _logger.Information(_memoryHandSaver.Context.HandActions.Count + " Hand Action(s) saved");
            _memoryHandSaver.Flush();
            _logger.Information("Memory Cache Flushed");
        }


        public void ReadFileInMemory(string file)
        {
      
                FileInfo fileInfo = new FileInfo(file);
                if (!_memoryHandSaver.HasTournament(fileInfo.Name, out var tournament))
                {
                    if (!_memoryHandReader.ReadTournament(file, out tournament))
                    {
                        return ;
                    }
                }

            string changes = ReadTail(file, tournament);
            ParseLinesInMemory(changes.Split(new char[] {'\r','\n' } , StringSplitOptions.RemoveEmptyEntries), tournament);
            _memoryHandSaver.SaveTournament(tournament);

        }
        private string ReadTail(string filename,Tournament tournament)
        {
            using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int bytesToRead = (int)(fs.Length - tournament.Bytes);
                fs.Seek(-bytesToRead, SeekOrigin.End);
                byte[] bytes = new byte[bytesToRead];
                fs.Read(bytes, 0, bytesToRead);
                string s = Encoding.Default.GetString(bytes);
                tournament.Bytes = (int)fs.Length;
                return s;
            }
        }

        private void ParseLinesInMemory(IEnumerable<string> lines, Tournament tournament)
        {
            var handState = HandState.Header;
            Hand currentHand = null;
            Dictionary<string, PlayerHand> playersInHand = new Dictionary<string, PlayerHand>();

            double potAmount = 0;
            int actionOrder = 0;
            DealtPotAmount handResult;
            HandAction handAction;
            var newLines = 0;
            for (int i = 0; i < lines.Count(); i++)
            {
                newLines += 1;
                var line = lines.ElementAt(i);

                if (IsSummary(line))
                {
                    _logger.Verbose("Summary :" + line);
                    handState = HandState.Summary;
                    continue;
                }
                else if (IsHeader(line))
                {
                    _memoryHandReader.UpdateHandHeader(line, out currentHand, tournament, newLines);
                    newLines = 0;
                    _memoryHandSaver.SaveHand(currentHand);
                    playersInHand = new Dictionary<string, PlayerHand>();
                    potAmount = 0;
                    actionOrder = 0;
                    handState = HandState.Table;
                    _logger.Verbose("Initialize New Hand:" + line);


                    continue;
                }
                switch (handState)
                {

                    case HandState.Table:
                        if (IsTable(line))
                        {
                            _logger.Verbose("Player in Table :" + line);
                            ReadHand(line, currentHand);
                    
                            handState += 1;
                        }
                        break;
                    case HandState.Seats:
                        if (IsSeat(line))
                        {
                            _logger.Verbose("Seated Player :" + line);
                            _memoryHandReader.ReadPlayerSeated(line, currentHand, out var player, out var playerHand);
                            _memoryHandSaver.SavePlayerHand(player, playerHand);
                            playersInHand.Add(player.PlayerName, playerHand);
                        }
                        else if (IsAnteBlinds(line))
                        {
                            _logger.Verbose("Blinds & Ante starts :" + line);
                            handState += 1;
                        }
                        break;
                    case HandState.AnteBlinds:

                        if (IsPreflop(line))
                        {
                            _logger.Verbose("Pre-flop starts :" + line);
                            handState += 1;
                        }
                        else if (IsDealtCards(line))
                        {
                            _memoryHandReader.ReadHandDealt(line, playersInHand);
                            ReadDealtCards(line);
                        }
                        else
                        {
                            handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.AnteBlinds);
                            potAmount = handResult.PotAmount;
                            _memoryHandSaver.SaveHandAction(handAction);
                            ReadBlindAnte(line);
                        }

                        break;
                    case HandState.PreFlop:
                        if (IsFlop(line))
                        {
                            _logger.Verbose("Flop starts :" + line);
                            ResetAmountPostedCurrentStage(playersInHand);
                            handState += 1;
                        }
                        else
                        {
                            handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.PreFlop);
                            potAmount = handResult.PotAmount;
                            _memoryHandSaver.SaveHandAction(handAction);
                            ReadPreFlopAction(line);
                        }
                        break;
                    case HandState.Flop:
                        if (IsTurn(line))
                        {
                            _logger.Verbose("Turn starts :" + line);
                            ResetAmountPostedCurrentStage(playersInHand);
                            handState += 1;
                        }
                        else
                        {
                            handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.Flop);
                            potAmount = handResult.PotAmount;
                            _memoryHandSaver.SaveHandAction(handAction);
                            ReadFlopAction(line);
                        }
                        break;
                    case HandState.Turn:
                        if (IsRiver(line))
                        {
                            handState += 1;
                            _logger.Verbose("River starts:" + line);
                            ResetAmountPostedCurrentStage(playersInHand);
                        }
                        else
                        {
                            handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.Turn);
                            potAmount = handResult.PotAmount;
                            _memoryHandSaver.SaveHandAction(handAction);
                            ReadTurnAction(line);
                        }
                        break;
                    case HandState.River:
                        if (IsShowdown(line))
                        {
                            _logger.Verbose("Showdown starts:" + line);
                            handState += 1;
                        }
                        else
                        {
                            handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.River);
                            potAmount = handResult.PotAmount;

                            _memoryHandSaver.SaveHandAction(handAction);
                            ReadRiverAction(line);
                        }
                        break;
                    case HandState.Showdown:
                        handResult = _memoryHandReader.ReadHandAction(line, playersInHand, currentHand, potAmount, out handAction, ++actionOrder, Constants.HandSteps.Showdown);
                        potAmount = handResult.PotAmount;
                        _memoryHandSaver.SaveHandAction(handAction);
                        ReadShowdown(line);
                        break;
                    case HandState.Summary:
                        if (IsEndOfHand(line))
                        {
                            _logger.Verbose("End Of Hand :" + line);
                        }
                        else if (IsTotalPot(line))
                        {
                            ReadTotalPot(line);
                        }
                        else if (IsBoardline(line))
                        {
                            ReadBoard(line);
                        }
                        else //Seat
                        {
                            ReadHandWinner(line);
                        }
                        break;
                }
            }
        }

        private void ResetAmountPostedCurrentStage(Dictionary<string, PlayerHand> playerHands)
        {
            foreach(var player in playerHands)
            {
                player.Value.PostedCurrentStage = 0;
            }
        }

        private void ReadHand(string line, Hand currentHand)
        {
            currentHand.DealerPosition = GetDealerPosition(line);
            var regex = new Regex(Constants.FilePattern.MaxPlayersPattern);
            var match = regex.Match(line);
            currentHand.MaxPlayers = int.Parse(match.Groups["MaxPlayers"].Value);
        }

        private void ReadPreFlopAction(string line)
        {
            _logger.Verbose("Pre-flop Action :" + line);
        }

        private void ReadBoard(string line)
        {
            _logger.Verbose("Board :" + line);
        }

        private void ReadTotalPot(string line)
        {
            _logger.Verbose("Total Pot :" + line);
        }

        private bool IsBoardline(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Board);
        }

        private bool IsTotalPot(string line)
        {
            return line.StartsWith(Constants.FileHeaders.TotalPot);
        }

        private void ReadHandWinner(string line)
        {
            _logger.Verbose("Hand Winner :" + line);
        }

        private void ReadDealtCards(string line)
        {

            _logger.Verbose("Dealts Cards :" + line);
        }

        private bool IsDealtCards(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Dealt);
        }
        

        private bool IsEndOfHand(string line)
        {
            return line.Length == 0;
        }

        private void ReadShowdown(string line)
        {
            _logger.Verbose("Showdown :" + line);
        }
    

        private void ReadRiverAction(string line)
        {
            _logger.Verbose("River Action :" + line);
        }

        private void ReadTurnAction(string line)
        {
            _logger.Verbose("Turn Action :" + line);
        }

        private bool IsSummary(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Summary);
        }
        private bool IsShowdown(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Showdown);
        }
        private bool IsRiver(string line)
        {
            return line.StartsWith(Constants.FileHeaders.River);
        }

        private bool IsTurn(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Turn);
        }
        private bool IsFlop(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Flop);
        }

        private void ReadFlopAction(string line)
        {
            _logger.Verbose("Flop Action :" + line);
        }

        private void ReadBlindAnte(string line)
        {
            _logger.Verbose("Blinds & Ante :" + line);
        }

        private bool IsPreflop(string line)
        {
            return line.StartsWith(Constants.FileHeaders.Preflop);
        }

        private bool IsAnteBlinds(string line)
        {
            return line.StartsWith(Constants.FileHeaders.AnteBlinds);
        }

        private bool IsSeat(string line)
        {
           
            if (!line.StartsWith(Constants.FileHeaders.Seat))
            {
                return false;
            }
            return true;
        }

        private int GetDealerPosition(string line)
        {
           var index= line.LastIndexOf("#");
            return int.Parse(line.Substring(index+1, 1));
        }

        private bool IsTable(string line)
        {
            return line.StartsWith(Constants.FileHeaders.TableBegin);
        }

        private bool IsHeader(string line)
        {
            return line.StartsWith(Constants.FileHeaders.HandBegin);
        }


    }
}
