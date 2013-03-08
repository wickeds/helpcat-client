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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;

namespace helpcat.Misc
{
    static class MessageParser
    {
        readonly static Dictionary<string, string> smilies = new Dictionary<string, string>()
        {
            //{ ":)", "smile" }
        };

        static Regex matchRegex;
        const string incompleteRegex = @"(?<url>https?://(?:[^ ]+)){0}|(?<plain>.+?)";//(?<newline>\r\n)|(?<newline>\n)|

        readonly static Regex alphanumRegex = new Regex(@"[^\w]", RegexOptions.Singleline);

        static MessageParser()
        {
            StringBuilder smilieRegex = new StringBuilder();

            foreach (KeyValuePair<string, string> pair in smilies)
            {
                smilieRegex.Append("|(?<emoticon_");
                smilieRegex.Append(pair.Value);
                smilieRegex.Append(">");
                smilieRegex.Append(Regex.Escape(pair.Key));
                smilieRegex.Append("?)");
            }

            matchRegex = new Regex(
                string.Format(
                incompleteRegex,
                smilieRegex.ToString())
                , RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static void Append(Paragraph paragraph, string message)
        {
            MatchCollection mC = matchRegex.Matches(message);
            string[] possible = matchRegex.GetGroupNames();

            foreach (Match m in mC)
            {
                foreach (string group in possible)
                {
                    if (group == "0")
                        continue;
                    string test = m.Groups[group].Value;
                    if (test != null && test != string.Empty)
                    {
                        if (group == "url")
                        {
                            try
                            {
                                Hyperlink link = new Hyperlink();
                                link.Inlines.Add(test);
                                link.NavigateUri = new Uri(test, UriKind.Absolute);
                                link.RequestNavigate += (object sender, RequestNavigateEventArgs e) =>
                                {
                                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                                    e.Handled = true;
                                };
                                paragraph.Inlines.Add(link);
                            }
                            catch (UriFormatException)
                            {
                                paragraph.Inlines.Add(test);
                            }
                        }
                        else if (group.IndexOf("emoticon_") == 0)
                        {
                            string cleanName = alphanumRegex.Replace(group.Substring(9), "");
                            Image img = new Image();
                            img.Width = 20;
                            img.Height = 20;
                            Uri u = new Uri(
                                string.Format("data/emoticons/{0}.png",
                                cleanName), UriKind.Relative);
                            try
                            {
                                BitmapImage src = new BitmapImage();
                                src.BeginInit();
                                src.UriSource = u;
                                src.CacheOption = BitmapCacheOption.OnLoad;
                                src.EndInit();
                                img.Source = src;
                                InlineUIContainer container = new InlineUIContainer(img);
                                paragraph.Inlines.Add(container);
                            }
                            catch (IOException)
                            {
                                paragraph.Inlines.Add(test);
                            }
                        }
                        else if (group == "plain")
                        {
                            paragraph.Inlines.Add(test);
                        }
                    }
                }
            }
        }
    }
}
