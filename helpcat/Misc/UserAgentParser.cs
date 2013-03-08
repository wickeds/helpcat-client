using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace helpcat.Misc
{
    static class UserAgentParser
    {
        public static HttpBrowserCapabilities Parse(string userAgent)
        {
            HttpBrowserCapabilities browser = new HttpBrowserCapabilities
            {
                Capabilities = new Hashtable { { string.Empty, userAgent } }
            };
            BrowserCapabilitiesFactory factory = new BrowserCapabilitiesFactory();
            factory.ConfigureBrowserCapabilities(new NameValueCollection(), browser);
            return browser;
        }
    }
}
