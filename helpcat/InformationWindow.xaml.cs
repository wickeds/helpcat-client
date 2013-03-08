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
using System.Net;
using helpcat.Misc;
using System.Web;

namespace helpcat
{
    /// <summary>
    /// Interaktionslogik für InformationWindow.xaml
    /// </summary>
    public partial class InformationWindow : MetroWindow
    {
        public InformationWindow(string userAgent, string location, IPAddress ipAddress)
        {
            InitializeComponent();
            HttpBrowserCapabilities caps = UserAgentParser.Parse(userAgent);
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);

            browserTextBlock.Text = caps.Browser;
            browserVersionTextBlock.Text = caps.Version;
            operatingSystemTextBlock.Text = caps.Platform;
            locationTextBlock.Text = location;
            ipAddressTextBlock.Text = ipAddress.ToString();
            hostnameTextBlock.Text = hostEntry.HostName;
            userAgentTextBlock.Text = userAgent;
        }
    }
}
