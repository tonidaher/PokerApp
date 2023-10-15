using System;
using System.Collections.Generic;
using System.Text;

namespace PokerApp.Domain
{
    public interface IFileWatcherService
    {
        delegate void NotifyNewHand(string fileName);
        void StartWatch();
        void StopWatch();
        event NotifyNewHand NewHand;
    }
}
