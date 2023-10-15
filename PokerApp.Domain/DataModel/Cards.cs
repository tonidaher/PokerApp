
using PokerApp.Domain.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PokerApp.Domain.DataModel
{
    public class Cards: IEquatable<Cards>
    {

        public Cards()
        {

        }
        public Cards(IList<string> cards)
        {
            int count = cards.Count;
            if( count>0) Card1 = cards[0];
            if (count > 1) Card2 = cards[1];
            if (count > 2) Card3 = cards[2];
            if (count > 3) Card4 = cards[3];
            if (count > 4) Card5 = cards[4];
        }
        public Cards(string card1, string card2)
        {
            Card1 = card1;
            Card2 = card2;
        }

        public Cards(string card1, string card2, string card3, string card4) : this(card1,card2)
        {
            Card3 = card3;
            Card4 = card4;
        }


        public string Card1 { get; set; }
        public string Card2 { get; set; }
        public string Card3 { get; set; }
        public string Card4 { get; set; }
        public string Card5 { get; set; }

        public bool Equals(Cards other)
        {
            if (other == null) return false;

            return Card1.EqualsIgnoreCase(other.Card1) && Card2.EqualsIgnoreCase(other.Card2) && Card3.EqualsIgnoreCase(other.Card3)
                && Card4.EqualsIgnoreCase(other.Card4) && Card5.EqualsIgnoreCase(other.Card5);
        }

        public override string ToString()
        {
            return string.Join(" ", Card1, Card2,Card3, Card4, Card5);
        }
    }
}
