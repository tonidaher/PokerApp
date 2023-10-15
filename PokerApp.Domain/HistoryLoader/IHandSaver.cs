using PokerApp.Domain.Dal;
using System;

namespace PokerApp.Domain
{
    public interface IHandSaver
    {
        void SaveTournament(TournamentDal t);
        void SaveHand(HandDal hand);
        void SaveHandAction(Join_HandActionDal hand);

        void Commit();
        void SavePlayerHand(Join_PlayerHandDal playerHand);
        void SavePlayer(PlayerDal player);
        bool HasTournament(string fileName, out TournamentDal tournament);
    }
}
