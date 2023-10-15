using PokerApp.Domain.DataModel;


namespace PokerApp.Domain
{
   public interface IPlayerPositionGetter
    {
       PointXY GetPlayerPosition(Rect windowSize,  int seatNumber, int heroSeatNumber, int maxPlayers);
    }
}
