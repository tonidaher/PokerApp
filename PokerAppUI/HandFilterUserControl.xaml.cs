using PokerApp.Domain.HandFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for HandFilterUserControl.xaml
    /// </summary>
    public partial class HandFilterUserControl : UserControl
    {

        public List<string> GameType { get; set; }
        public List<string> BuyIn { get; set; }
        public delegate void FilterChanged();
        public event FilterChanged OnFilterChanged;

        private HandFilterFactory _filterFactory;
        public HandFilterUserControl()
        {
            InitializeComponent();
            var context = MainWindow.ContextProvider.GetNewContext();
            var tournaments = context.GetTournaments();
            GameType = context.GetTournaments().Select(x => x.TournamentType).Distinct().ToList();
            gameTypeListBox.ItemsSource = GameType;
            BuyIn = context.GetTournaments().Select(x => x.BuyIn ?? "0").Distinct().ToList();
            buyInListBox.ItemsSource = BuyIn;
        }

        private void CurrentSessionCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.CurrentSession, null);
            Update();
        }

        private void Update()
        {
            OnFilterChanged?.Invoke();
        }

        private void CurrentSessionCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            _filterFactory.RemoveFilter(PokerApp.Domain.Common.Constants.FilterType.CurrentSession, null);
            Update();
        }

        private void GameTypeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var filterParam = ((CheckBox)sender).Content.ToString();
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.GameType, filterParam);
            Update();
        }

        private void BuyInCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var filterParam = ((CheckBox)sender).Content.ToString();     
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.BuyIn, filterParam);
            Update();
        }

        private void lastNHandsTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var filterParam = ((TextBox)sender).Text;
            if (!int.TryParse(filterParam, out _)) { return; }
            
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.LastnHands, filterParam);
            Update();
        }

        private void OnlyRealMondayCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.OnlyRealMoney);
            Update();
        }

        private void nowButton_Click(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Now;
        }

        private void resestTimeButton_Click(object sender, RoutedEventArgs e)
        {
            _filterFactory.RemoveFilter(PokerApp.Domain.Common.Constants.FilterType.DateTime);
            Update();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _filterFactory.AddFilter(PokerApp.Domain.Common.Constants.FilterType.DateTime, datePicker.SelectedDate.Value.ToString());
            Update();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _filterFactory = (HandFilterFactory)DataContext;
        }
    }
}
