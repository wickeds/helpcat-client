using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace helpcat.Controls.TabControlEx
{
    class CloseableTab : TabItem
    {
        CloseableHeader closeableTabHeader;

        public CloseableTab()
        {
            closeableTabHeader = new CloseableHeader();
            this.Header = closeableTabHeader;
            ((CloseableHeader)this.Header).closeButton.MouseDown += closeButton_MouseDown;
        }

        public void Close()
        {
            OnTabPageClosed(new TabPageClosedEventArgs());
            ((TabControl)this.Parent).Items.Remove(this);
        }

        void closeButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public string Title
        {
            set
            {
                ((CloseableHeader)this.Header).titleLabel.Content = value;
            }
        }

        bool selected = false;

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            selected = true;
            ((CloseableHeader)this.Header).closeButton.Opacity = 0.5;
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            base.OnUnselected(e);
            selected = false;
            ((CloseableHeader)this.Header).closeButton.Opacity = 0.25;
        }
        
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            ((CloseableHeader)this.Header).closeButton.Opacity = selected ? 0.75 : 0.5;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            ((CloseableHeader)this.Header).closeButton.Opacity = selected ? 0.5 : 0.25;
        }

        public delegate void TabPageClosedEventHandler(Object sender, TabPageClosedEventArgs e);

        public class TabPageClosedEventArgs : EventArgs
        {
            public object UserState
            {
                get;
                set;
            }

            public TabPageClosedEventArgs()
            {

            }

            public TabPageClosedEventArgs(object userState)
            {
                UserState = userState;
            }
        }

        public event TabPageClosedEventHandler TabPageClosed;

        protected virtual void OnTabPageClosed(TabPageClosedEventArgs e)
        {
            if (TabPageClosed != null)
                TabPageClosed(this, e);
        }
    }
}
