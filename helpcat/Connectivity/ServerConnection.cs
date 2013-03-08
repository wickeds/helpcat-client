using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Specialized;
using RestSharp;
using WickedFramework.RestSharpExtensions;

namespace helpcat.Connectivity
{
    public class ServerConnection
    {
        RestClient restClient;
        const string genericCallEndpoint = "api/call.php?action={action}";

        public ServerConnection(string baseUrl)
        {
            restClient = new RestClient(baseUrl);
            restClient.CookieContainer = new CookieContainer();
        }

        public async Task Authentificate(string username, string password)
        {
            RestRequest req = new RestRequest("api/auth.php", Method.POST);
            req.AddParameter("name", username);
            req.AddParameter("password", password);
            var resp = await restClient.ExecuteTaskAsync<AuthResponse>(req);

            if (!resp.Data.Success)
            {
                throw new AuthentificationException(resp.Data.Error.Message);
            }
        }

        public async Task<NotificationsFetchResponse> PullNotifications()
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "notifications.fetch");
            var res = await restClient.ExecuteTaskAsync<NotificationsFetchResponse>(req);
            if (res.Data.Success)
            {
                return res.Data;
            }
            else
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task<ChatJoinResponse> JoinChat(int id)
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "chat.join");
            req.AddParameter("id", id);
            var res = await restClient.ExecuteTaskAsync<ChatJoinResponse>(req);
            if (res.Data.Success)
            {
                return res.Data;
            }
            else
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task LeaveChat(int id)
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "chat.leave");
            req.AddParameter("id", id);
            var res = await restClient.ExecuteTaskAsync<ChatLeaveResponse>(req);
            if (!res.Data.Success)
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task SendMessage(int id, string text)
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "chat.sendMessage");
            req.AddParameter("id", id);
            req.AddParameter("text", text);
            var res = await restClient.ExecuteTaskAsync<ChatSendMessageResponse>(req);
            if (!res.Data.Success)
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task<ChatFetchMessagesResponse> FetchMessages(int id)
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "chat.fetchMessages");
            req.AddParameter("id", id);
            var res = await restClient.ExecuteTaskAsync<ChatFetchMessagesResponse>(req);

            if (res.Data.Success)
            {
                return res.Data;
            }
            else
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task<SettingsGetResponse> GetSettings()
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "settings.get");
            var res = await restClient.ExecuteTaskAsync<SettingsGetResponse>(req);

            if (res.Data.Success)
            {
                return res.Data;
            }
            else
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task SetSettings(NameValueCollection settings)
        {
            RestRequest req = new RestRequest(genericCallEndpoint, Method.POST);
            req.AddUrlSegment("action", "settings.set");
            var res = await restClient.ExecuteTaskAsync<SettingsSetResponse>(req);

            if (!res.Data.Success)
            {
                throw new ApiException(res.Data.Error.Message);
            }
        }

        public async Task<string> GetLocation(IPAddress ipAddress)
        {
            return "undefined";
        }
    }
}
