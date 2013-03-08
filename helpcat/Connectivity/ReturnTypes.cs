using System;
using System.Collections.Generic;

namespace helpcat.Connectivity
{
    public class Response
    {
        public bool Success { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class AuthResponse : Response
    {
    }

    public class NotificationsFetchResponse : Response
    {
        public List<Notification> Notifications { get; set; }
    }

    public class ChatJoinResponse : Response
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public string UserAgent { get; set; }
    }

    public class ChatLeaveResponse : Response
    {
    }

    public class ChatFetchMessagesResponse : Response
    {
        public bool Open { get; set; }
        public bool Active { get; set; }
        public List<Message> Messages { get; set; }
    }

    public class ChatSendMessageResponse : Response
    {
    }

    public class SettingsGetResponse : Response
    {
        public Settings Settings;
    }

    public class SettingsSetResponse : Response
    {
    }

    public class SessionEndResponse : Response
    {

    }

    public class Notification
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public int Target { get; set; }
    }

    public class Message
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public int Timestamp { get; set; }
        public int Rank { get; set; }
    }

    public class Settings
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }
}
