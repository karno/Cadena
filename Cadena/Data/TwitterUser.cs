using System;
using System.Linq;
using System.Text;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterUser
    {
        public TwitterUser()
        {
            ScreenName = String.Empty;
        }

        public TwitterUser(dynamic json)
        {
            Id = ((string)json.id_str).ParseLong();
            ScreenName = ParsingExtension.ResolveEntity(json.screen_name);
            Name = ParsingExtension.ResolveEntity(json.name ?? String.Empty);
            Description = ParsingExtension.ResolveEntity(json.description ?? String.Empty);
            Location = ParsingExtension.ResolveEntity(json.location ?? String.Empty);
            Url = json.url;
            IsDefaultProfileImage = json.default_profile_image;
            ProfileImageUri = ((string)json.profile_image_url).ParseUri();
            ProfileBackgroundImageUri = ((string)json.profile_background_image_url).ParseUri();
            if (json.profile_banner_url())
            {
                ProfileBannerUri = ((string)json.profile_banner_url).ParseUri();
            }
            IsProtected = json["protected"];
            IsVerified = json.verified;
            IsTranslator = json.is_translator;
            IsContributorsEnabled = json.contributors_enabled;
            IsGeoEnabled = json.geo_enabled;
            StatusesCount = (long)((double?)json.statuses_count ?? default(double));
            FollowingsCount = (long)((double?)json.friends_count ?? default(double));
            FollowersCount = (long)((double?)json.followers_count ?? default(double));
            FavoritesCount = (long)((double?)json.favourites_count ?? default(double));
            ListedCount = (long)((double?)json.listed_count ?? default(double));
            Language = json.lang;
            CreatedAt = ((string)json.created_at).ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
            if (json.entities())
            {
                if (json.entities.url())
                {
                    UrlEntities = Enumerable.ToArray(TwitterEntity.ParseEntities(json.entities.url));
                }
                if (json.entities.description())
                {
                    DescriptionEntities = Enumerable.ToArray(TwitterEntity.ParseEntities(json.entities.description));
                }
            }
        }

        public TwitterUser(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            ScreenName = ParsingExtension.ResolveEntity(json["screen_name"].AsString());
            Name = ParsingExtension.ResolveEntity(json["name"].AsString() ?? String.Empty);
            Description = ParsingExtension.ResolveEntity(json["description"].AsString() ?? String.Empty);
            Location = ParsingExtension.ResolveEntity(json["location"].AsString() ?? String.Empty);
            Url = json["url"].AsString();
            IsDefaultProfileImage = json["default_profile_image"].AsBoolean();
            ProfileImageUri = json["profile_image_url"].AsString().ParseUri();
            ProfileBackgroundImageUri = json["profile_background_image_url"].AsString().ParseUri();
            var banner = json["profile_banner_url"].AsString();
            if (banner != null)
            {
                ProfileBannerUri = banner.ParseUri();
            }
            IsProtected = json["protected"].AsBoolean();
            IsVerified = json["verified"].AsBoolean();
            IsTranslator = json["is_translator"].AsBoolean();
            IsContributorsEnabled = json["contributors_enabled"].AsBoolean();
            IsGeoEnabled = json["geo_enabled"].AsBoolean();
            StatusesCount = json["statuses_count"].AsLong();
            FollowingsCount = json["friends_count"].AsLong();
            FollowersCount = json["followers_count"].AsLong();
            FavoritesCount = json["favourites_count"].AsLong();
            ListedCount = json["listed_count"].AsLong();
            Language = json["lang"].AsString();
            CreatedAt = json["created_at"].AsString().ParseDateTime(ParsingExtension.TwitterDateTimeFormat);
            var entities = json["entities"].AsObject();
            if (entities != null)
            {
                var urls = entities["url"].AsString();
                var descs = entities["description"].AsString();
                if (urls != null)
                {
                    UrlEntities = TwitterEntity.ParseEntities(urls).ToArray();
                }
                if (descs != null)
                {
                    DescriptionEntities = TwitterEntity.ParseEntities(descs).ToArray();
                }
            }
        }

        public const string TwitterUserUrl = "https://twitter.com/{0}";
        public const string FavstarUserUrl = "http://favstar.fm/users/{0}";
        public const string TwilogUserUrl = "http://twilog.org/{0}";

        /// <summary>
        /// Exactly Numeric ID of user. (PRIMARY KEY)
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// ScreenName ( sometimes also call @ID ) of user.
        /// </summary>
        [NotNull]
        public string ScreenName { get; }

        /// <summary>
        /// Name for the display of user.
        /// </summary>
        [CanBeNull]
        public string Name { get; }

        /// <summary>
        /// Description of user, also calls &quot;Bio&quot;
        /// </summary>
        [CanBeNull]
        public string Description { get; }

        /// <summary>
        /// Location of user.
        /// </summary>
        [CanBeNull]
        public string Location { get; }

        /// <summary>
        /// Url of user. <para />
        /// Warning: This property, named URL but, may not be exactly URI.
        /// </summary>
        [CanBeNull]
        public string Url { get; }

        /// <summary>
        /// Profile image is default or not.
        /// </summary>
        public bool IsDefaultProfileImage { get; }

        /// <summary>
        /// Profile image of user.
        /// </summary>
        [CanBeNull]
        public Uri ProfileImageUri { get; }

        /// <summary>
        /// Profile background image of user.
        /// </summary>
        [CanBeNull]
        public Uri ProfileBackgroundImageUri { get; }

        /// <summary>
        /// Profile background image of user.
        /// </summary>
        [CanBeNull]
        public Uri ProfileBannerUri { get; }

        /// <summary>
        /// Flag for check protected of user.
        /// </summary>
        public bool IsProtected { get; }

        /// <summary>
        /// Flag of user is verified by twitter official.
        /// </summary>
        public bool IsVerified { get; }

        /// <summary>
        /// Flag of user works as translator.
        /// </summary>
        public bool IsTranslator { get; }

        /// <summary>
        /// Flag of user using &quot;Writers&quot;
        /// </summary>
        public bool IsContributorsEnabled { get; }

        /// <summary>
        /// Flag of user using &quot;geo&quot; feature.
        /// </summary>
        public bool IsGeoEnabled { get; }

        /// <summary>
        /// Amount of tweets of user.
        /// </summary>
        public long StatusesCount { get; }

        /// <summary>
        /// Amount of friends(a.k.a followings) of user.
        /// </summary>
        public long FollowingsCount { get; }

        /// <summary>
        /// Amount of followers of user.
        /// </summary>
        public long FollowersCount { get; }

        /// <summary>
        /// Amount of favorites of user.
        /// </summary>
        public long FavoritesCount { get; }

        /// <summary>
        /// Amount of listed by someone of user.
        /// </summary>
        public long ListedCount { get; }

        /// <summary>
        /// Language of user
        /// </summary>
        public string Language { get; }

        /// <summary>
        /// Created time of user
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Entities of user url
        /// </summary>
        [CanBeNull]
        public TwitterEntity[] UrlEntities { get; }

        /// <summary>
        /// Entities of user description
        /// </summary>
        [CanBeNull]
        public TwitterEntity[] DescriptionEntities { get; }

        [NotNull]
        public string UserPermalink
        {
            get { return String.Format(TwitterUserUrl, ScreenName); }
        }

        [NotNull]
        public string FavstarUserPermalink
        {
            get { return String.Format(FavstarUserUrl, ScreenName); }
        }

        [NotNull]
        public string TwilogUserPermalink
        {
            get { return String.Format(TwilogUserUrl, ScreenName); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id == ((TwitterUser)obj).Id;
        }

        [NotNull]
        public string GetEntityAidedUrl()
        {
            var ou = UrlEntities?.FirstOrDefault(u => u.EntityType == EntityType.Urls)?.OriginalUrl;
            return ou ?? Url ?? String.Empty;
        }

        [NotNull]
        public string GetEntityAidedDescription(bool showFullUrl = false)
        {
            var builder = new StringBuilder();
            var escaped = Description ?? String.Empty;
            TwitterEntity prevEntity = null;
            var de = DescriptionEntities ?? new TwitterEntity[0];
            foreach (var entity in de.OrderBy(e => e.StartIndex))
            {
                var pidx = 0;
                if (prevEntity != null)
                    pidx = prevEntity.EndIndex;
                if (pidx < entity.StartIndex)
                {
                    // output raw
                    builder.Append(ParsingExtension.ResolveEntity(escaped.Substring(pidx, entity.StartIndex - pidx)));
                }
                switch (entity.EntityType)
                {
                    case EntityType.Hashtags:
                        builder.Append("#" + entity.DisplayText);
                        break;
                    case EntityType.Urls:
                        builder.Append(showFullUrl && !String.IsNullOrEmpty(entity.OriginalUrl)
                                           ? ParsingExtension.ResolveEntity(entity.OriginalUrl)
                                           : ParsingExtension.ResolveEntity(entity.DisplayText));
                        break;
                    case EntityType.Media:
                        builder.Append(showFullUrl && !String.IsNullOrEmpty(entity.MediaUrl)
                                           ? ParsingExtension.ResolveEntity(entity.MediaUrl)
                                           : ParsingExtension.ResolveEntity(entity.DisplayText));
                        break;
                    case EntityType.UserMentions:
                        builder.Append("@" + entity.DisplayText);
                        break;
                }
                prevEntity = entity;
            }
            if (prevEntity == null)
            {
                builder.Append(ParsingExtension.ResolveEntity(escaped));
            }
            else if (prevEntity.EndIndex < escaped.Length)
            {
                builder.Append(ParsingExtension.ResolveEntity(
                    escaped.Substring(prevEntity.EndIndex, escaped.Length - prevEntity.EndIndex)));
            }
            return builder.ToString();
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}