using PokerApp.Domain;
using PokerApp.Domain.Common;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Data
{
    public class MemoryHandContext: IMemoryHandContext
    {
        public MemoryHandContext()
        {
            Tournaments = new List<Tournament>();
            Players = new List<Player>();
            Hands = new List<Hand>();
            Actions = new List<Action>();
            HandActions = new List<HandAction>();
            PlayerHands = new List<PlayerHand>();
            CurrentPlayers = new List<Player>();

        }
        public List<Hand> Hands { get; set; }
        public List<Player> Players { get; set; }

        public List<Tournament> Tournaments { get; set; }
        public List<Action> Actions { get; set; }

        public List<HandAction> HandActions { get; set; }
        public List<PlayerHand> PlayerHands { get; set; }

        public List<Player> CurrentPlayers { get; set; }

        public void Flush()
        {
          //  Tournaments.Clear();
            Hands.Clear();
            Players.Clear();
            HandActions.Clear();
            PlayerHands.Clear();
        }
    }
}
