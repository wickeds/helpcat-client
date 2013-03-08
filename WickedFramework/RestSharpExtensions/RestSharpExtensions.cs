using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace WickedFramework.RestSharpExtensions
{
    public static class RestSharpExtensions
    {
        public static Task<IRestResponse<T>> ExecuteTaskAsync<T>(this RestClient client, IRestRequest req) where T : new()
        {
            TaskCompletionSource<IRestResponse<T>> tcs = new TaskCompletionSource<IRestResponse<T>>();
            client.ExecuteAsync<T>(req, r =>
            {
                if (r.ErrorException == null)
                {
                    tcs.SetResult(r);
                }
                else
                {
                    tcs.SetException(r.ErrorException);
                }
            });
            return tcs.Task;
        }
    }
}
