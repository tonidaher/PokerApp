using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IMemoryHandContext
    {
        public List<Hand> Hands { get; set; }
        public List<Player> Players { get; set; }
        public List<Tournament> Tournaments { get; set; }
        public List<Action> Actions { get; set; }
        public List<HandAction> HandActions { get; set; }
        public List<PlayerHand> PlayerHands { get; set; }
        public List<Player> CurrentPlayers { get; set; }
        public void Flush();
    }
}
