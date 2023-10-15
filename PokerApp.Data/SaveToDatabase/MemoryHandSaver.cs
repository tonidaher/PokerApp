using Microsoft.EntityFrameworkCore.Internal;
using PokerApp.Domain;
using PokerApp.Domain.Dal;
using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;

namespace PokerApp.Data.SaveToDatabase
{
    public class MemoryHandSaver : IMemoryHandSaver
    {
        private object handsLocker = new object();
        private object playerLocker = new object();

        public MemoryHandSaver(MemoryHandContext context)
        {
            Context = context;
        }

        public IMemoryHandContext Context { get; set; }
   


        public bool HasTournament(string fileName, out Tournament tournament)
        {
            tournament = Context.Tournaments.FirstOrDefault(x => x.Filename.Equals(fileName));
            return tournament != null;
        }

        public void SaveHand(Hand hand)
        {
            lock (handsLocker)
            {
                if (Context.Hands.Contains(hand))
                {
                    RemoveHands(new List<Hand>() { hand });
                }
                Context.Hands.Add(hand);
            }
        }

        public void SaveHandAction(HandAction handAction)
        {
            Context.HandActions.Add(handAction);
        }

        public void SavePlayerHand(Player player, PlayerHand playerHand)
        {
            lock (playerLocker)
            {
                if (!Context.Players.Contains(player))
                {
                    Context.Players.Add(player);
                }
            }
            Context.PlayerHands.Add(playerHand);
        }

        public void SaveTournament(Tournament t)
        {
            lock (handsLocker)
            {
                if (Context.Tournaments.Contains(t))
                {
                    var handsToRemove = Context.Hands.Where(x => x.TournamentFileName == t.Filename && x.Line > t.Lines);
                    if (!handsToRemove.Any()) { return; }
                    RemoveHands(handsToRemove.ToList());
                }
                else
                {
                    Context.Tournaments.Add(t);
                }
            }
        }

        private void RemoveHands(List<Hand> handsToRemove)
        {
            var handIdToRemove = handsToRemove.Select(y => y.HandId);
            Context.Hands.RemoveAll(x=>handIdToRemove.Contains(x.HandId) );
            Context.PlayerHands.RemoveAll(x=> handIdToRemove.Contains(x.HandId));
            Context.HandActions.RemoveAll(x => handIdToRemove.Contains(x.HandId));
        }

        public void Flush()
        {
            Context.Flush();
        }
    }
}
