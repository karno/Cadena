using System;
using Cadena.Meteor;
using JetBrains.Annotations;

namespace Cadena.Data.Streams.Events
{
    public class StreamAccessInformationEvent : StreamEvent<AccessInformation, AccessInformationEvents>
    {
        internal const string AccessRevokedEventKey = "access_revoked";
        internal const string AccessUnrevokedEventKey = "access_unrevoked";

        public StreamAccessInformationEvent(TwitterUser source, TwitterUser target,
            AccessInformation targetObject, string rawEvent, DateTime createdAt)
            : base(source, target, targetObject, ToEnumEvent(rawEvent), rawEvent, createdAt)
        {
        }

        public static AccessInformationEvents ToEnumEvent(string eventStr)
        {
            switch (eventStr)
            {
                case AccessRevokedEventKey:
                    return AccessInformationEvents.AccessRevoked;

                case AccessUnrevokedEventKey:
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

        public AccessInformation(JsonValue json)
        {
            Token = json["token"].AsString();
            ClientApplication = new ClientApplication(json["client_application"]);
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

        public ClientApplication(JsonValue json)
        {
            Id = json["id"].AsLong();
            Name = json["name"].AsString();
            Url = json["url"].AsString();
            ConsumerKey = json["consumer_key"].AsString();
        }
    }

    public enum AccessInformationEvents
    {
        Unknown,
        AccessRevoked,
        AccessUnrevoked,
    }
}