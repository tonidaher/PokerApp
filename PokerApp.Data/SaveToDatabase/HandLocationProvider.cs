using PokerApp.Domain;

namespace PokerApp.Data
{
    public class HandLocationProvider : IHandLocationProvider

    { 
        private readonly string _handLocation;
        public HandLocationProvider (string handLocation)
    {
            _handLocation = handLocation;
    }
        public string GetHandLocation()
        {
            return _handLocation;
        }
    }
}
