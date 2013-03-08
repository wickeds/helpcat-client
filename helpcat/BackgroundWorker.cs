using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using helpcat.Connectivity;
using helpcat.Misc;

namespace helpcat
{
    class BackgroundWorker
    {
        ServerConnection serverConn;

        public BackgroundWorker(ServerConnection serverConn)
        {
            this.serverConn = serverConn;
        }

        public void Run()
        {
            System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = helpcat.Properties.Resources.notifyicon;
            notifyIcon.Text = "helpcat";
            notifyIcon.Visible = true;
            notifyIcon.Click += notifyIcon_Click;

            System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            System.Windows.Forms.MenuItem exitMenuItem = new System.Windows.Forms.MenuItem("Exit");
            exitMenuItem.Click += exitMenuItem_Click;
            contextMenu.MenuItems.Add(exitMenuItem);

            System.Windows.Forms.MenuItem chatMenuItem = new System.Windows.Forms.MenuItem("Chat (Only for debugging)");
            chatMenuItem.Click += chatMenuItem_Click;
            contextMenu.MenuItems.Add(chatMenuItem);

            System.Windows.Forms.MenuItem settingsMenuItem = new System.Windows.Forms.MenuItem("Settings");
            settingsMenuItem.Click += settingsMenuItem_Click;
            contextMenu.MenuItems.Add(settingsMenuItem);

            notifyIcon.ContextMenu = contextMenu;

            DispatcherTimer pullTimer = new DispatcherTimer();
            pullTimer.Interval = TimeSpan.FromSeconds(2);
            pullTimer.Tick += pullTimer_Tick;
            pullTimer.Start();
        }

        void settingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow settings = WindowTools.Search<SettingsWindow>();
            if (settings == null)
            {
                settings = new SettingsWindow(serverConn);
                settings.Show();
            }
            settings.Focus();
        }

        void chatMenuItem_Click(object sender, EventArgs e)
        {
            ChatWindow chat = new ChatWindow(serverConn);
            chat.Show();
        }

        void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        void notifyIcon_Click(object sender, EventArgs e)
        {

        }

        async void pullTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                NotificationsFetchResponse res = await serverConn.PullNotifications();
                foreach (Notification notification in res.Notifications)
                {
                    if (notification.Type == "new_customer")
                    {
                        NotificationWindow n = new NotificationWindow(Localization.Strings.NewCustomer, notification.Text);
                        n.NotificationClicked += (object _sender, NotificationWindow.NotificationClickEventArgs _e) =>
                        {
                            ChatWindow.NewTab(notification.Target, serverConn);
                        };
                        n.Show();
                    }
                    else if (notification.Type == "broadcast")
                    {
                        NotificationWindow n = new NotificationWindow(Localization.Strings.NewCustomer, notification.Text);
                        n.Show();
                    }
                }
            }
            catch (
#if DEBUG
                NotImplementedException _e
#else
                Exception _e
#endif
                )
            {
                NotificationWindow n = new NotificationWindow("Exception raised", _e.Message);
                n.Show();
            }
        }
    }
}
