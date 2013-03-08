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
using System.Windows.Interop;
using System.Windows.Threading;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        DateTime startupTime = DateTime.Now;
        int slot = NotificationStacker.ReserveSlot();

        public NotificationWindow(string title, string content)
        {
            InitializeComponent();
            double delta = (double)slot * this.Height;
            this.Left = SystemParameters.WorkArea.Width - this.Width;
            this.Top = SystemParameters.WorkArea.Height - this.Height - delta;
            this.titleLabel.Content = title;
            this.contentLabel.Text = content;

            

            DispatcherTimer animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromSeconds((double)1 / (double)30);
            animationTimer.Tick += animationTimer_Tick;
            animationTimer.Start();
        }

        void animationTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan timeLeft = TimeSpan.FromSeconds(Properties.Settings.Default.NotificationTimeout) - (DateTime.Now - startupTime);
            if (timeLeft.TotalSeconds > 0)
            {
                double rectMod = timeLeft.TotalSeconds / Properties.Settings.Default.NotificationTimeout;
                timeoutProgressRectangle.Width = this.Width * rectMod;
            }
            else
            {
                this.Close();
            }
        }

        private void Window_SourceInitialized_1(object sender, EventArgs e)
        {
            WindowInteropHelper interopHelper = new WindowInteropHelper(this);
            Natives.EnableShadow(interopHelper.Handle);
            Natives.MakeUnfocusable(interopHelper.Handle);
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            NotificationStacker.CancelSlot(slot);
        }

        private void Window_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            OnNotificationClicked(new NotificationClickEventArgs());
            this.Close();
        }

        #region Event Declarations
        public delegate void NotificationClickEventHandler(Object sender, NotificationClickEventArgs e);

        public class NotificationClickEventArgs : EventArgs
        {
            public NotificationClickEventArgs()
            {
            }
        }

        public event NotificationClickEventHandler NotificationClicked;

        protected virtual void OnNotificationClicked(NotificationClickEventArgs e)
        {
            if (NotificationClicked != null)
                NotificationClicked(this, e);
        }
        #endregion
    }
}
