using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;

namespace helpcat.Connectivity
{
    static class WebClientExtensions
    {
        private static SHA512CryptoServiceProvider sha512 = new SHA512CryptoServiceProvider();

        public static async Task<byte[]> UploadMultipartFormDataTaskAsync(this WebClient webClient, Uri address, Dictionary<string, object> content)
        {
            MemoryStream requestBody = new MemoryStream();
            StreamWriter requestWriter = new StreamWriter(requestBody, Encoding.ASCII);

            string boundary = GenerateBoundary();

            foreach (KeyValuePair<string, object> entry in content)
            {
                requestWriter.Write(string.Format("--{0}\r\n", boundary));
                requestWriter.Write(string.Format("content-disposition: form-data; name=\"{0}\"\r\n", entry.Key));
                requestWriter.Write("content-type: text/plain;charset=utf-8\r\n");
                requestWriter.Write("content-transfer-encoding: base64\r\n");
                requestWriter.Write("\r\n");
                requestWriter.Write(Convert.ToBase64String(Encoding.UTF8.GetBytes((string)entry.Value)));
                requestWriter.Write("\r\n");
                requestWriter.Flush();
            }

            requestBody.Flush();

            System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(requestBody.ToArray()));
            return await webClient.UploadDataTaskAsync(address, "POST", requestBody.ToArray());
        }

        private static string GenerateBoundary()
        {
            return Convert.ToBase64String(sha512.ComputeHash(BitConverter.GetBytes(DateTime.Now.Ticks)));
        }
    }
}
