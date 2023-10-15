using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IHistoryHandContextProvider
    {
        IHistoryHandContext GetNewContext();
    }
}
