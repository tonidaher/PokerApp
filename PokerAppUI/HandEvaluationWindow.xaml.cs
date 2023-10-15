using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using PokerApp.Domain;
using System.Threading;
using System.Threading.Tasks;
using PokerApp.Domain.DataModel;
using PokerApp.Domain.HandEvaluation;
using PokerAppUI.Icons;
using PokerAppUI.Struct;
using System.Linq;
using System.Windows.Input;

namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for HandEvaluationWindow.xaml
    /// </summary>
    public partial class HandEvaluationWindow : Window
    {
        private CardRecognizer _cardRecognizer;
        private Cards HoleCards, BoardCards;
        private IHoldemHandEvaluator _handEvaluator;
        private TournamentStruct _tournament;
        private string _accountName;
        public HandEvaluationWindow(TournamentStruct tournament, PokerApp.Domain.Rect position, string accountName)
        {

            InitializeComponent();
            Top = position.Bottom;
            Left = position.Left;
            _tournament = tournament;
            Title = _tournament.TournamentName + "(" +_tournament.TournamentId+ ")";
            _accountName = accountName;
            Init();
        }

        private void Init()
        {
            IScreenshotTaker screenshotTaker = new ScreenshotTaker();
            IWindowPositionGetter windowPositionGetter = new WindowPositionGetter();
            _handEvaluator = new HoldemHandEvaluator();
            _cardRecognizer = new CardRecognizer(windowPositionGetter, screenshotTaker);
            Visibility = Visibility.Visible;
            Task.Factory.StartNew(() =>
            {
                RecognizeCards();
            });
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        public void RecognizeCards()
        {
         
            Cards newHoleCards;
            Cards newBoardCards;

            newHoleCards = _cardRecognizer.RecognizeHoleCards(_tournament.TournamentId, _tournament.TournamentName);

            newBoardCards = _cardRecognizer.RecognizeBoardCards(_tournament.TournamentId, _tournament.TournamentName);

            var cardsChanged = CardsChanged(newHoleCards, newBoardCards);
            if (cardsChanged)
            {
                HoleCards = newHoleCards;
                BoardCards = newBoardCards;
                if (HoleCards != null)
                {
                    DisplayHoleCards();
                   
                }
                if (BoardCards != null) DisplayBoardCards();
            }
            if (cardsChanged && ValidForEval(newHoleCards, newBoardCards))
            {
                EvaluateHand();
            }
            Thread.Sleep(3000);
            RecognizeCards();
        }

        private void DisplayBoardCards()
        {
            Dispatcher.Invoke(() =>
            {
                ImageBoardCard1.Source = ImageHelper.LoadImage(BoardCards.Card1);
                ImageBoardCard2.Source = ImageHelper.LoadImage(BoardCards.Card2);
                ImageBoardCard3.Source = ImageHelper.LoadImage(BoardCards.Card3);
                ImageBoardCard4.Source = ImageHelper.LoadImage(BoardCards.Card4);
                ImageBoardCard5.Source = ImageHelper.LoadImage(BoardCards.Card5);
            });
        }

        private bool ValidForEval(Cards holeCards, Cards boardCards)
        {
            return holeCards != null && boardCards != null;
        }

        private bool CardsChanged(Cards newHoleCards, Cards newBoardCards)
        {
            return (newHoleCards != null && !newHoleCards.Equals(HoleCards))
                || (newBoardCards != null && !newBoardCards.Equals(BoardCards));
        }

        private void opponents_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValidForEval(HoleCards, BoardCards))
            {
                EvaluateHand();
            }
        }

        private void EvaluateHand()
        {
            Dispatcher.Invoke(() =>
            {
                var evaluationResult = _handEvaluator.EvaluateHand(HoleCards.ToString(), BoardCards.ToString(), (int)opponentsSlider.Value);
                handEvaluatorDisplayer.Update(Dispatcher, evaluationResult);
            });
        }

        private void DisplayHoleCards()
        {
            Dispatcher.Invoke(() =>
            {
                ImageHoleCard1.Source = ImageHelper.LoadImage(HoleCards.Card1);
                ImageHoleCard2.Source = ImageHelper.LoadImage(HoleCards.Card2);
                UpdateCardsMetrics();
            });
        }

        public void UpdateCardsMetrics()
        {
            if(HoleCards!=null)
            gridTest.DataContext = PlayerHudManager.CardMetricCalculator.GetHandMetrics(_accountName, HoleCards.Card1 + HoleCards.Card2);
        }

        internal void Dispose()
        {
            Dispatcher.Invoke(() =>
            {
                Visibility = Visibility.Collapsed;
                Close();
            });
        }

    }
}
