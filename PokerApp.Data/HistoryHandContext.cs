using Microsoft.EntityFrameworkCore;
using PokerApp.Domain;
using PokerApp.Domain.Dal;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess
{
    public class HistoryHandContext : DbContext, IHistoryHandContext
    {
        public DbSet<HandDal> Hands { get; set; }
        public DbSet<PlayerDal> Players { get; set; }

        public DbSet<TournamentDal> Tournaments { get; set; }
        public DbSet<Dim_ActionDal> Dim_Actions { get; set; }

        public DbSet<Join_HandActionDal> HandActions { get; set; }
        public DbSet<Join_PlayerHandDal> PlayerHands { get; set; }

        public DbSet<CurrentPlayersDal> CurrentPlayers { get; set; }

        public DbSet<MetricThresholdDal> MetricThresholds { get; set; }

        private readonly string _connectionString;

        public HistoryHandContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Join_HandActionDal>()
                .HasKey(c => new { c.HandId, c.PlayerName, c.ActionOrder });


            modelBuilder.Entity<Join_PlayerHandDal>()
                .HasKey(c => new { c.HandId, c.PlayerName});
      
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        public IList<Join_PlayerHandDal> GetPlayerHands(string player)
        {
            return PlayerHands.Where(x => x.PlayerName.Equals(player)).Include(x=>x.Hand).Include(x=>x.Hand.Tournament).ToList();
        }
        public IList<CurrentPlayersDal> GetCurrentPlayers()
        {
            return CurrentPlayers.ToList();
        }

        public IList<TournamentDal> GetTournaments()
        {
            return Tournaments.ToList();
        }

        public IList<MetricThresholdDal> GetMetricThresholds()
        {
            return MetricThresholds.ToList();
        }
    }
}
