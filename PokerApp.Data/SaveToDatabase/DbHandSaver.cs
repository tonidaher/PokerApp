using DataAccess;
using Microsoft.EntityFrameworkCore;
using PokerApp.Domain;
using PokerApp.Domain.Dal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerApp.Data
{
   public class DbHandSaver : IHandSaver

    {
        private IHistoryHandContextProvider _contextProvider;
        private HistoryHandContext _context;

        public DbHandSaver(IHistoryHandContextProvider contextProvider)
        {
            _contextProvider = contextProvider;
            _context = (HistoryHandContext)_contextProvider.GetNewContext();
            
        }

        public void Commit()
        {

            _context.SaveChanges();
            _context.Dispose();
            _context = (HistoryHandContext)_contextProvider.GetNewContext();
         
        }

        public void SaveHand(HandDal hand)
        {
            if (_context.Hands.Contains(hand))
            {
                RemoveHands(new List<HandDal>() {hand }) ;
            }
            _context.Hands.Add(hand);
        }

        public void SavePlayerHand(Join_PlayerHandDal playerHand)
        {
            
            _context.PlayerHands.Add(playerHand);
        }

        public void SavePlayer(PlayerDal player)
        {
            if (!_context.Players.Contains(player))
            {
                _context.Players.Add(player);
            }
        }

        public void SaveTournament(TournamentDal t)
        {
            if (!_context.Tournaments.Contains(t))
            {
                _context.Tournaments.Add(t);
            }     
        }

        private void RemoveHands(List<HandDal> handsToRemove)
        {
            var handIdToRemove = handsToRemove.Select(y => y.HandId);
            var playerHandsToRemove = _context.PlayerHands.Where(x => handIdToRemove.Contains(x.HandId));
            var handActionToRemove = _context.HandActions.Where(x => handIdToRemove.Contains(x.HandId));
            _context.Hands.RemoveRange(handsToRemove);
            _context.PlayerHands.RemoveRange(playerHandsToRemove);
            _context.HandActions.RemoveRange(handActionToRemove);
            _context.SaveChanges();
        }

        public void SaveHandAction(Join_HandActionDal handAction)
        {
            _context.HandActions.Add(handAction);
        }

        public bool HasTournament(string fileName, out TournamentDal tournament)
        {
           tournament = _context.Tournaments.FirstOrDefault(x => x.Filename == fileName);
            return tournament != null;
        }
    }
}
