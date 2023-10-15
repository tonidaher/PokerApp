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
using System.Windows.Shapes;

namespace PokerAppUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string PlayerName { get; set; }
        public string HandsFolder { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
            textBoxPlayerName.Text = Properties.Settings.Default.AccountName;
            textBoxHandsFolder.Text = Properties.Settings.Default.HandsFolder;
            HandsFolder = textBoxHandsFolder.Text;
            KeyDown += LoginWindow_KeyDown;
        }

        private void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
           if(e.Key == Key.Enter)
            {
                OnLogin();
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            OnLogin();

        }

        private void OnLogin()
        {
            PlayerName = textBoxPlayerName.Text;
            HandsFolder = textBoxHandsFolder.Text;
            Properties.Settings.Default.HandsFolder = HandsFolder;
            Properties.Settings.Default.AccountName = PlayerName;
            Properties.Settings.Default.Save();
            DialogResult = true;
        }
    }
}
