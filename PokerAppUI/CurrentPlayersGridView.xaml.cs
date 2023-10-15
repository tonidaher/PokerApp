using System.Windows;
using System.Windows.Controls;


namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for CurrentPlayersGridView.xaml
    /// </summary>
    public partial class CurrentPlayersGridView : UserControl
    {
        private PlayerHudManager _hudManager;
        public CurrentPlayersGridView()
        {
            InitializeComponent();
        }

        private void HandFilterUserControl_OnFilterChanged()
        {
           _hudManager.Update();
           _hudManager.OnFilterUpdate();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _hudManager = (PlayerHudManager)DataContext;
            handFilterUserControl.DataContext = PlayerHudManager.Calculator.FilterFactory;
            handFilterUserControl.OnFilterChanged += HandFilterUserControl_OnFilterChanged;
        }
    }
}
