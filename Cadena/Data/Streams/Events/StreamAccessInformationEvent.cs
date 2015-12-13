using System;
using JetBrains.Annotations;

namespace Cadena.Data.Streams.Events
{
    public class StreamAccessInformationEvent : StreamEvent<AccessInformation, AccessInformationEvents>
    {
        public StreamAccessInformationEvent(TwitterUser source, TwitterUser target,
            AccessInformation targetObject, string rawEvent, DateTime createdAt)
            : base(source, target, targetObject, ToEnumEvent(rawEvent), rawEvent, createdAt)
        {
        }

        public static AccessInformationEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr)
            {
                case "access_revoked":
                    return AccessInformationEvents.AccessRevoked;
                case "access_unrevoked":
                    return AccessInformationEvents.AccessUnrevoked;
                default:
                    return AccessInformationEvents.Unknown;
            }
        }
    }

    public class AccessInformation
    {
        [NotNull]
        public string Token { get; }

        [NotNull]
        public ClientApplication ClientApplication { get; }

        public AccessInformation(dynamic json)
        {
            Token = json.token;
            ClientApplication = new ClientApplication(json.client_application);
        }
    }

    public class ClientApplication
    {
        public long Id { get; }

        [NotNull]
        public string Name { get; }

        [NotNull]
        public string Url { get; }

        [NotNull]
        public string ConsumerKey { get; }

        public ClientApplication(dynamic json)
        {
            Id = json.id;
            Name = json.name;
            Url = json.url;
            ConsumerKey = json.consumer_key;
        }
    }

    public enum AccessInformationEvents
    {
        Unknown,
        AccessRevoked,
        AccessUnrevoked,
    }
}
