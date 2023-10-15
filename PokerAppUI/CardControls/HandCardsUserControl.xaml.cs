using PokerApp.Domain.DataModel;
using PokerAppUI.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PokerAppUI.CardControls
{
    /// <summary>
    /// Interaction logic for HandCardsUserControl.xaml
    /// </summary>
    public partial class HandCardsUserControl : UserControl
    {
        public HandCardsUserControl()
        {
            InitializeComponent();

        }

        // for this code image needs to be a project resource

        public void DisplayCards(Dispatcher dispatcher,IList<Cards> inputCards)
        {
            dispatcher.Invoke(() =>
            {
                var outputCards = inputCards.Select(x => new CardsDisplay() { Card1 = ImageHelper.LoadImage(x.Card1), Card2 = ImageHelper.LoadImage(x.Card2) }).ToList();

                cards.Visibility = Visibility.Visible;
                cards.ItemsSource = outputCards;
            });
        }

        public void DisplayBoard(Dispatcher dispatcher, Cards boardCards)
        {
            dispatcher.Invoke(() =>
            {
                var outputCards =  new CardsDisplay()  {Card1 = ImageHelper.LoadImage(boardCards.Card1), Card2 = ImageHelper.LoadImage(boardCards.Card2) , Card3 = ImageHelper.LoadImage(boardCards.Card3) 
                , Card4 = ImageHelper.LoadImage(boardCards.Card4), Card5= ImageHelper.LoadImage(boardCards.Card5) };

                cards.Visibility = Visibility.Visible;
                cards.ItemsSource = new List<CardsDisplay>() { outputCards };
            });
        }

        public void Display()
        {
            var playerName = ((PlayerMetrics)DataContext).PlayerName;
            var lastCards = PlayerHudManager.Calculator.GetLastNCards(playerName).Select(x=>new CardsDisplay() { Card1 = ImageHelper.LoadImage(x.Card1), Card2 = ImageHelper.LoadImage(x.Card2) });

   
            Visibility = Visibility.Visible;
            cards.ItemsSource = lastCards;
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
            cards.ItemsSource = null;
        }
    }
    public class CardsDisplay
    {
        public BitmapImage Card1 { get; set; }
        public BitmapImage Card2 { get; set; }
        public BitmapImage Card3 { get; set; }
        public BitmapImage Card4 { get; set; }
        public BitmapImage Card5 { get; set; }
    }
   
}
