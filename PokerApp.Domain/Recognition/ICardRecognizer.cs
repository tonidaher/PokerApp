using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PokerApp.Domain
{
   public interface ICardRecognizer
    {
        Cards RecognizeHoleCards(string tournamentId, string tournamentName);
        Cards RecognizeBoardCards(string tournamentId, string tournamentName);
    }
}
