using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain.DataModel
{
    public class Player : IEquatable<Player>
    {
        public string PlayerName { get; set; }

        public Player(string player)
        {
            PlayerName = player;
        }

        public bool Equals(Player other)
        {
            return PlayerName.Equals(other.PlayerName);
        }
    }
}
