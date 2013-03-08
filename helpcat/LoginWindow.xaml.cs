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
using helpcat.Dialogs;
using WickedDeployment;
using System.Reflection;
using helpcat.Localization;
using System.Net;
using System.Diagnostics;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MetroWindow
    {
        ServerConnection serverConn;
        const string updateUri = "https://github.com/wickeds/helpcat-client";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_SourceInitialized_1(object sender, EventArgs e)
        {
            usernameTextBox.Text = Properties.Settings.Default.LastUsername;
            endpointTextBox.Text = Properties.Settings.Default.LastEndpoint;
            
            if (endpointTextBox.Text.Length == 0)
                endpointTextBox.Focus();
            else if (usernameTextBox.Text.Length == 0)
                usernameTextBox.Focus();
            else
                passwordPasswordBox.Focus();

            await Task.Run(() =>
            {
                UpdateClient uc = new UpdateClient();
                if (uc.UpdateAvailable(Assembly.GetExecutingAssembly().GetName().Version))
                {
                    updateNoticeTextBlock.Dispatcher.Invoke(() => updateNoticeTextBlock.Visibility = System.Windows.Visibility.Visible);
                }
            });
        }

        private void updateStartHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(updateUri);
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (endpointTextBox.Text.Length == 0)
            {
                TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.NoEndpointSelected, Localization.Strings.PleaseSelectAnEndpoint, TaskDialogButtons.OK, TaskDialogIcon.Error);
                return;
            }

            serverConn = new ServerConnection(endpointTextBox.Text + "/");

            loginFormStackPanel.IsEnabled = false;
            loginProgressIndicator.Visibility = System.Windows.Visibility.Visible;

            try
            {
                await serverConn.Authentificate(usernameTextBox.Text, passwordPasswordBox.Password);

                Properties.Settings.Default.LastUsername = usernameTextBox.Text;
                Properties.Settings.Default.LastEndpoint = endpointTextBox.Text;
                Properties.Settings.Default.Save();

                this.Hide();

                BackgroundWorker bWorker = new BackgroundWorker(serverConn);
                bWorker.Run();
            }
            catch (WebException)
            {
                loginFormStackPanel.IsEnabled = true;
                loginProgressIndicator.Visibility = System.Windows.Visibility.Hidden;
                passwordPasswordBox.Password = string.Empty;
                passwordPasswordBox.Focus();
                TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.InvalidUsername, Localization.Strings.PleaseRetryUsername, TaskDialogButtons.OK, TaskDialogIcon.Error);
            }
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                loginButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

        private void MetroWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            Activate();
        }
    }
}
