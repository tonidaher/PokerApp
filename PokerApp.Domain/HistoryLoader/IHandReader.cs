
using PokerApp.Domain.Common;
using PokerApp.Domain.Dal;
using System.Collections.Generic;

namespace PokerApp.Domain
{
    public interface IHandReader
    {
       bool ReadTournament(string fileName, out TournamentDal tournament);
        void UpdateHandHeader(string line, out HandDal hand, TournamentDal tournament, int newLines);
        void ReadPlayerSeated(string line, HandDal hand, out PlayerDal player, out Join_PlayerHandDal playerHand);
        DealtPotAmount ReadHandAction(string line, Dictionary<string,Join_PlayerHandDal> playersInHand, string handId, double potAmount, out Join_HandActionDal handAction, int actionOrder, string step);
    }
}
