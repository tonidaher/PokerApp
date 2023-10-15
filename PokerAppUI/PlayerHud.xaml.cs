using PokerApp.Domain.DataModel;
using System.Windows;
using System.Windows.Input;

namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for PlayerHud.xaml
    /// </summary>
    public partial class PlayerHud : Window
    {
        private bool _cardsDisplayed = false;
        public PlayerHud(PlayerMetrics player, Point pos)
        {
            InitializeComponent();
            Left = pos.X;
            Top = pos.Y;
            Update(player);
        }

        private void SetValues(PlayerMetrics player)
        {
            DataContext = player;
            Visibility = Visibility.Visible;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
                if (e.ChangedButton == MouseButton.Left)
                    DragMove();       
        }

        internal void Update(PlayerMetrics player)
        {
            Dispatcher.Invoke(() =>
            {
                SetValues(player);
            });
        }

        internal void Dispose()
        {
            Dispatcher.Invoke(() =>
            {
                Visibility = Visibility.Collapsed;
            Close();
            });
        }


        private void DisplayCardsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cardsDisplayed)
            {
                Height = 70;
                displayCardsButton.Content = "-";
                cards.Hide();
            }
            else
            {
                Height = 500;
                displayCardsButton.Content = "+";
                cards.Display();
            }
            _cardsDisplayed = !_cardsDisplayed;
        }
    }

}
