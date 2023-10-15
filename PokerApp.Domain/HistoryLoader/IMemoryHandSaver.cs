using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IMemoryHandSaver
    {
        IMemoryHandContext Context { get; set; }
        void SaveTournament(Tournament t);
        void SaveHand(Hand hand);
        void SaveHandAction(HandAction hand);

        void SavePlayerHand(Player player, PlayerHand playerHand);

        bool HasTournament(string fileName, out Tournament tournament);
        void Flush();
    }
}
