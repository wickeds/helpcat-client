using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using helpcat.Connectivity;
using helpcat.Controls.TabControlEx;
using helpcat.Dialogs;
using helpcat.Misc;
using System.Threading.Tasks;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : MetroWindow
    {
        ServerConnection serverConn;
        Dictionary<int, ChatControl> activeChats = new Dictionary<int, ChatControl>();

        DispatcherTimer messageRetrieverTimer = new DispatcherTimer();

        bool allowClosing = false;

        public ChatWindow(ServerConnection serverConn)
        {
            InitializeComponent();
            this.serverConn = serverConn;

            messageRetrieverTimer = new DispatcherTimer();
            messageRetrieverTimer.Interval = TimeSpan.FromSeconds(1);
            messageRetrieverTimer.Tick += messageRetrieverTimer_Tick;
            messageRetrieverTimer.Start();
        }

        public static void NewTab(int id, ServerConnection serverConn)
        {
            ChatWindow instance = WindowTools.Search<ChatWindow>();

            if (instance == null)
            {
                instance = new ChatWindow(serverConn);
                instance.Show();
            }

            try
            {
                instance.NewTab(id);
            }
            catch (Exception)
            {
                instance.Close();
                throw;
            }
        }

        public async void NewTab(int id)
        {
            ChatJoinResponse res = await serverConn.JoinChat(id);

            CloseableTab tab = new CloseableTab();
            tab.Style = FindResource("CakeTabItem") as Style;
            tab.Title = res.Name; ;
            tab.TabPageClosed += async (sender, e) =>
            {
                try
                {
                    await serverConn.LeaveChat(id);
                }
                catch (Exception _e)
                {
                    if (_e is WebException || _e is ArgumentException || _e is ApiException)
                    {
                        TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.ChatLeaveFailed, _e.Message, TaskDialogButtons.OK, TaskDialogIcon.Warning);
                    }
                    else
                    {
                        throw;
                    }
                }

                if (chatTabControl.Items.Count == 0)
                {
                    allowClosing = true;
                    this.Close();
                }
            };

            string location = await serverConn.GetLocation(IPAddress.Parse(res.Addr));

            ChatControl con = new ChatControl();
            con.nameLbl.Text = res.Name;
            con.ipLbl.Text = res.Addr;
            con.countryLbl.Text = location;

            tab.Content = con;

            activeChats.Add(id, con);
            chatTabControl.Items.Add(tab);

            chatTabControl.SelectedItem = tab;

            con.inputTb.KeyDown += async (object sender, KeyEventArgs e) =>
            {
                if (e.Key == Key.Enter)
                {
                    e.Handled = true;
                    TextBox textBox = ((TextBox)sender);
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        int caret = textBox.CaretIndex;
                        textBox.Text = ((TextBox)sender).Text.Insert(caret, Environment.NewLine);
                        textBox.CaretIndex = caret + 1;
                    }
                    else
                    {
                        string text = textBox.Text;
                        if (text.Length > 0)
                        {
                            textBox.Text = "";

                            await SendMessage(id, text, con);
                        }
                    }
                }
            };


            con.informationButton.Click += (object sender, RoutedEventArgs e) =>
            {
                InformationWindow info = new InformationWindow(res.UserAgent, location, IPAddress.Parse(res.Addr));
                info.Owner = this;
                info.Show();
            };


            AppendFormattedText(con.contentRtb, con.contentParagraph, "You are connected to the customer.", Colors.Red);
            AppendLineBreak(con.contentRtb, con.contentParagraph);

            this.Focus();
            Natives.BringToFront(this);

            if (Properties.Settings.Default.EnableGreetingMessage)
                await SendMessage(id, Properties.Settings.Default.GreetingMessage, con);
        }

        async Task SendMessage(int id, string text, ChatControl con)
        {
            try
            {
                await serverConn.SendMessage(id, text);
            }
            catch (Exception _e)
            {
                if (_e is WebException || _e is ArgumentException || _e is ApiException)
                {
                    // show error message, catch other errors todo
                    // translation todo
                    AppendFormattedText(con.contentRtb, con.contentParagraph, "Couldn't deliver the message because an internal error occurred.", Colors.Red);
                    AppendLineBreak(con.contentRtb, con.contentParagraph);
                }
                else
                {
                    throw;
                }
            }
        }

        async void messageRetrieverTimer_Tick(object sender, EventArgs e)
        {
            messageRetrieverTimer.Stop();

            bool needSound = false;

            List<int> removalQueue = new List<int>();
            Dictionary<int, ChatControl> clone;

            lock (activeChats)
            {
                clone = new Dictionary<int, ChatControl>(activeChats);
            }

            foreach (KeyValuePair<int, ChatControl> pair in clone)
            {
                int id = pair.Key;
                ChatControl con = pair.Value;

                try
                {
                    ChatFetchMessagesResponse res = await serverConn.FetchMessages(id);


                    if (res.Success && res.Open && !res.Active)
                    {
                        AppendFormattedText(con.contentRtb, con.contentParagraph, Localization.Strings.ConversatonTimedOut, Colors.Red);
                        AppendLineBreak(con.contentRtb, con.contentParagraph);
                        con.inputTb.IsEnabled = false;
                        removalQueue.Add(id);
                        continue;
                    }
                    else if (res.Success && !res.Open)
                    {
                        AppendFormattedText(con.contentRtb, con.contentParagraph, Localization.Strings.ConversationEnded, Colors.Red);
                        AppendLineBreak(con.contentRtb, con.contentParagraph);
                        con.inputTb.IsEnabled = false;
                        removalQueue.Add(id);
                        continue;
                    }
                    else if (res.Success)
                    {
                        foreach (Message message in res.Messages)
                        {
                            DateTime timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                            timestamp = timestamp.AddSeconds(message.Timestamp);

                            string timestampFormatted = timestamp.ToLocalTime().ToLongTimeString();

                            if (message.Rank == 0 && !needSound && !(Natives.IsForegroundWindow(this)/* && ((TabItem)chatTabControl.SelectedItem).Content == con*/)) // why?
                                needSound = true;

                            AppendFormattedText(con.contentRtb, con.contentParagraph, timestampFormatted + " ", Colors.Gray);
                            AppendFormattedText(con.contentRtb, con.contentParagraph, message.Name, message.Rank == 0 ? Colors.Blue : (message.Rank == 1 ? Colors.Red : Colors.Gray));
                            AppendFormattedText(con.contentRtb, con.contentParagraph, ": ", Colors.Black);
                            AppendParsedText(con.contentRtb, con.contentParagraph, message.Text);
                            AppendLineBreak(con.contentRtb, con.contentParagraph);
                        }
                    }
                }
                catch (Exception _e)
                {
                    if (_e is WebException || _e is ArgumentException || _e is ApiException) // complete?
                    {
                        // todo: schöner und weniger platzraubend machen :/
                        AppendFormattedText(con.contentRtb, con.contentParagraph, _e.ToString(), Colors.Red);
                        AppendLineBreak(con.contentRtb, con.contentParagraph);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            foreach (int id in removalQueue)
                activeChats.Remove(id);
            
            if (needSound)
                SoundManager.Play("data/newmessage.wav");

            messageRetrieverTimer.Start();
        }

        private void AppendParsedText(RichTextBox richTextBox, Paragraph paragraph, string text)
        {
            MessageParser.Append(paragraph, text);
            richTextBox.ScrollToEnd();
        }

        private void AppendFormattedText(RichTextBox richTextBox, Paragraph paragraph, string text, Color color)
        {
            Run run = new Run(text);
            run.Foreground = new SolidColorBrush(color);
            paragraph.Inlines.Add(run);

            richTextBox.ScrollToEnd();
        }

        private void AppendLineBreak(RichTextBox richTextBox, Paragraph paragraph)
        {
            LineBreak lb = new LineBreak();
            paragraph.Inlines.Add(lb);

            richTextBox.ScrollToEnd();
        }

        private void MetroWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClosing)
            {
                e.Cancel = true;

                if (TaskDialogEx.ShowEx(this, Localization.Strings.helpcat, Localization.Strings.DoYouReallyWantToCloseTheChatWindow, Localization.Strings.DoYouReallyWantToCloseTheChatWindowExplanation, TaskDialogButtons.Yes | TaskDialogButtons.No, TaskDialogIcon.Warning) == TaskDialogResult.Yes)
                {
                    chatTabControl.IsEnabled = false;
                    List<TabItem> open = new List<TabItem>();
                    foreach (TabItem item in chatTabControl.Items)
                        open.Add(item);

                    foreach (TabItem item in open)
                    {
                        if (item is CloseableTab)
                        {
                            ((CloseableTab)item).Close();
                        }
                        else
                        {
                            chatTabControl.Items.Remove(item);
                        }
                    }
                }
            }
        }

        private void MetroWindow_Loaded_1(object sender, RoutedEventArgs e)
        {
            Activate();
        }
    }
}
