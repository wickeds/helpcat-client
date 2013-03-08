using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using WickedFramework.Serialization;
using System.Net;
using System.Net.Cache;

namespace WickedFramework.Connectivity
{
    public class RestClient
    {
        static UTF8Encoding utf8Enc = new UTF8Encoding(false, false);

        static WebClient GetWebClient()
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = utf8Enc;
            webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            return webClient;
        }

        public static dynamic Download(Uri uri)
        {
            WebClient web = GetWebClient();
            byte[] buffer = web.DownloadData(uri);
            string response = utf8Enc.GetString(buffer);
            return JsonDeserializer.Deserialize(response);
        }

        public static async Task<dynamic> DownloadTaskAsync(Uri uri)
        {
            WebClient web = GetWebClient();
            byte[] buffer = await web.DownloadDataTaskAsync(uri);
            string response = utf8Enc.GetString(buffer);
            return JsonDeserializer.Deserialize(response);
        }

        public static dynamic UploadValues(Uri uri, NameValueCollection parameters)
        {
            WebClient web = GetWebClient();
            byte[] buffer = web.UploadValues(uri, "POST", parameters);
            string response = utf8Enc.GetString(buffer);
            return JsonDeserializer.Deserialize(response);
        }

        public static async Task<dynamic> UploadValuesTaskAsync(Uri uri, NameValueCollection parameters)
        {
            WebClient web = GetWebClient();
            byte[] buffer = await web.UploadValuesTaskAsync(uri, "POST", parameters);
            string response = utf8Enc.GetString(buffer);
            return JsonDeserializer.Deserialize(response);
        }
    }
}
