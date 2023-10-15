using PokerApp.Domain.Common;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IMemoryHandReader
    {
        bool ReadTournament(string fileName, out Tournament tournament);
        void UpdateHandHeader(string line, out Hand hand, Tournament tournament, int newLines);
        void ReadPlayerSeated(string line, Hand hand, out Player player, out PlayerHand playerHand);

        DealtPotAmount ReadHandAction(string line, Dictionary<string, PlayerHand> playersInHand, Hand hand, double potAmount, out HandAction handAction, int actionOrder, string step);

        void ReadHandDealt(string line, Dictionary<string, PlayerHand> playersInHand);
    }
}
