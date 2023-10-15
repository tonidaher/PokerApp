using DataAccess;
using PokerApp.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Data
{
    public class HistoryHandContextProvider : IHistoryHandContextProvider
    {
        private string _connectionString;
        public HistoryHandContextProvider(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IHistoryHandContext GetNewContext()
        {
            var context = new HistoryHandContext(_connectionString);
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            
            return context;
        }
    }
}
