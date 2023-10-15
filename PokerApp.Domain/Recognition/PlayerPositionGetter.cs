using PokerApp.Domain.DataModel;
using System.Drawing;


namespace PokerApp.Domain
{
    public class PlayerPositionGetter : IPlayerPositionGetter
    {

        private const double heroX = 0.597;
        private const double heroY = 0.74;
        private double[] SixHandedX = new double[6] { heroX, 0.0229, 0.0229, 0.597, 0.797, 0.797 };
        private double[] SixHandedY = new double[6] { heroY, 0.5512, 0.32, 0.073, 0.32, 0.5512 };
        private double[] FiveHandedX = new double[5] { heroX, 0.038, 0.325,0.78, 0.22 };
        private double[] FiveHandedY = new double[5] { heroY, 0.413, 0.09, 0.78, 0.413 };
        private double[] EightHandedX = new double[8] { 0.572, 0.0318, 0.02, 0.265, 0.557, 0.838, 0.84, 0.825 };
        private double[] EightHandedY = new double[8] { 0.75, 0.568, 0.265, 0.0775, 0.0775 , 0.112, 0.265, 0.568 };

        private int[] NineHanded = new int[5] { 1, 1, 1, 1, 1 };
        public PointXY GetPlayerPosition(Rect windowSize, int seatNumber, int heroSeatNumber, int maxPlayers)
        {
            var absolutPosition = (seatNumber - heroSeatNumber + maxPlayers) % maxPlayers;
            switch (maxPlayers)
            {
                case 6:
                    return new PointXY() { X = (int)(windowSize.Left + ((windowSize.Right-windowSize.Left) * SixHandedX[absolutPosition])), Y = (int)(windowSize.Bottom * SixHandedY[absolutPosition]) };
                case 5:
                    return new PointXY() { X = (int)(windowSize.Left + ((windowSize.Right-windowSize.Left) * FiveHandedX[absolutPosition])), Y = (int)(windowSize.Bottom * FiveHandedY[absolutPosition]) };
                case 8:
                    return new PointXY() { X = (int)(windowSize.Left + ((windowSize.Right-windowSize.Left) * EightHandedX[absolutPosition])), Y = (int)(windowSize.Bottom * EightHandedY[absolutPosition]) };

                default:
                    return new PointXY(10, 10);
            }
              
        }
    }
}
