using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Cache;
using System.Dynamic;
using WickedFramework.Connectivity;
using WickedFramework.Tools;

namespace WickedDeployment
{
    public class UpdateClient
    {
        static readonly UTF8Encoding utf8Enc = new UTF8Encoding(false, false);

        readonly Uri manifestUri = new Uri("https://raw.github.com/wickeds/helpcat-client/master/version.json");
        Version remoteVersion;

        static WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = utf8Enc;
            webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            return webClient;
        }

        /// <summary>
        /// Initializes a new instance of the UpdateClient class.
        /// </summary>
        /// <param name="channelName">The release channel that should be used when checking for updates</param>
        public UpdateClient()
        {
            try
            {
                dynamic result = RestClient.Download(manifestUri);
            }
            catch (WebException)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// The manifest for the highest version available reported by the update server
        /// </summary>
        public Version RemoteVersion
        {
            get
            {
                return remoteVersion;
            }
        }

        /// <summary>
        /// Checks whether the current application version requires an update
        /// </summary>
        /// <param name="version">The current application version</param>
        /// <returns>True if an update is required, false otherwise</returns>
        public bool UpdateAvailable(Version version)
        {
            return version < RemoteVersion;
        }
    }
}
