using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using helpcat.Connectivity;
using System.Collections.Specialized;
using helpcat.Misc;
using helpcat.Dialogs;
using System.Net;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : MetroWindow
    {
        ServerConnection serverConn;

        public SettingsWindow(ServerConnection serverConn)
        {
            InitializeComponent();
            this.serverConn = serverConn;

            automaticallyGreetCustomersCheckBox.IsChecked = Properties.Settings.Default.EnableGreetingMessage;
            greetingMessageTextBox.Text = Properties.Settings.Default.GreetingMessage;
        }

        private async void MetroWindow_SourceInitialized_1(object sender, EventArgs e)
        {
            try
            {
                SettingsGetResponse res = await serverConn.GetSettings();

                emailTextBox.Text = res.Settings.Email;
                displayNameTextBox.Text = res.Settings.DisplayName;

                personalDataGroupBox.IsEnabled = true;
            }
            catch (Exception _e)
            {
                if (_e is WebException || _e is ArgumentException || _e is ApiException)
                {
                    TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.CouldntRetrieveRemoteSettings, _e.Message, TaskDialogButtons.OK, TaskDialogIcon.Error);
                }
                else
                {
                    throw;
                }
            }
        }

        private async void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            Properties.Settings.Default.EnableGreetingMessage = automaticallyGreetCustomersCheckBox.IsChecked == true ? true : false;
            Properties.Settings.Default.GreetingMessage = greetingMessageTextBox.Text;
            Properties.Settings.Default.Save();

            try
            {
                await serverConn.SetSettings(new NameValueCollection()
                {
                    { "email", emailTextBox.Text },
                    { "password", passwordPasswordBox.Password },
                    { "display_name", displayNameTextBox.Text }
                });
            }
            catch (Exception _e)
            {
                if (_e is WebException || _e is ArgumentException || _e is ApiException)
                {
                    // punkt weg
                    TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.CouldntUpdateRemoteSettings, _e.Message, TaskDialogButtons.OK, TaskDialogIcon.Error);
                }
                else
                {
                    throw;
                }
            }

            this.Close();
        }

        private void discardButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void VerifyFields()
        {
            saveButton.IsEnabled = emailTextBox.Text.Contains('@') && (passwordPasswordBox.Password.Length == 0 || passwordPasswordBox.Password.Length >= 6) && displayNameTextBox.Text.Length > 0;
        }

        private void emailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyFields();
        }

        private void passwordPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            VerifyFields();
        }

        private void displayNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            VerifyFields();
        }
    }
}
