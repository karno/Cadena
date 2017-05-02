using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cadena.Api.Parameters
{
    public sealed class ListParameter : ParameterBase
    {
        public long? ListId { get; }

        public long? OwnerId { get; }

        [CanBeNull]
        public string OwnerScreenName { get; }

        [CanBeNull]
        public string Slug { get; }

        public ListParameter(long listId)
        {
            ListId = listId;
            OwnerId = null;
            OwnerScreenName = null;
            Slug = null;
        }

        public ListParameter(long ownerId, string slug)
        {
            ListId = null;
            OwnerId = ownerId;
            OwnerScreenName = null;
            Slug = slug;
        }

        public ListParameter(string ownerScreenName, string slug)
        {
            ListId = null;
            OwnerId = null;
            OwnerScreenName = ownerScreenName;
            Slug = slug;
        }

        public override void SetDictionary(IDictionary<string, object> target)
        {
            target["list_id"] = ListId;
            target["owner_id"] = OwnerId;
            target["owner_screen_name"] = OwnerScreenName;
            target["slug"] = Slug;
        }
    }
}