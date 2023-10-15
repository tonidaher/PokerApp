using PokerApp.Domain.DataModel;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for HandEvaluationResultUserControl.xaml
    /// </summary>
    public partial class HandEvaluationResultUserControl : UserControl
    {

        public HandEvaluationResult Result {get;set;}

        public HandEvaluationResultUserControl()
        {
            InitializeComponent();
            Result = new HandEvaluationResult();
            DataContext = Result;
        }

        public void Update(Dispatcher dispatcher, HandEvaluationResult result)
        {
            dispatcher.Invoke(() =>
            {
                Result = result;
                DataContext = Result;
                Visibility = Visibility.Visible;
            });
        }
    }
}
