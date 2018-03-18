using System;
using System.Linq;
using Cadena.Data.Entities;
using Cadena.Meteor;
using Cadena.Util;
using JetBrains.Annotations;

namespace Cadena.Data
{
    public class TwitterUser
    {
        public TwitterUser(JsonValue json)
        {
            Id = json["id_str"].AsString().ParseLong();
            ScreenName = ParsingExtension.ResolveEntity(json["screen_name"].AsString());
            Name = ParsingExtension.ResolveEntity(json["name"].AsString());
            Description = ParsingExtension.ResolveEntity(json["description"].AsString());
            Location = ParsingExtension.ResolveEntity(json["location"].AsString());
            Url = json["url"].AsString();
            IsDefaultProfileImage = json["default_profile_image"].AsBooleanOrNull() ?? true;
            ProfileImageUri = json["profile_image_url"].AsString().ParseUri();
            ProfileBackgroundImageUri = json["profile_background_image_url"].AsString().ParseUri();
            ProfileBannerUri = json["profile_banner_url"].AsString().ParseUri();
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
            var entities = json["entities"];
            var urls = entities["url"];
            var descs = entities["description"];

            UrlEntities = !urls.IsNull
                ? TwitterEntity.ParseEntities(urls).OfType<TwitterUrlEntity>().ToArray()
                : new TwitterUrlEntity[0];

            DescriptionEntities = !descs.IsNull
                ? TwitterEntity.ParseEntities(descs).ToArray()
                : new TwitterEntity[0];
        }

        public TwitterUser(long id, [NotNull] string screenName, [CanBeNull] string name,
            [CanBeNull] string description, [CanBeNull] string location, [CanBeNull] string url,
            bool isDefaultProfileImage, [CanBeNull] Uri profileImageUri,
            [CanBeNull] Uri profileBackgroundImageUri, [CanBeNull] Uri profileBannerUri,
            bool isProtected, bool isVerified, bool isTranslator, bool isContributorsEnabled, bool isGeoEnabled,
            long statusesCount, long followingsCount, long followersCount, long favoritesCount, long listedCount,
            string language, DateTime createdAt, [NotNull] TwitterUrlEntity[] urlEntities,
            [NotNull] TwitterEntity[] descriptionEntities)
        {
            Id = id;
            ScreenName = screenName ?? throw new ArgumentNullException(nameof(screenName));
            Name = name;
            Description = description;
            Location = location;
            Url = url;
            IsDefaultProfileImage = isDefaultProfileImage;
            ProfileImageUri = profileImageUri;
            ProfileBackgroundImageUri = profileBackgroundImageUri;
            ProfileBannerUri = profileBannerUri;
            IsProtected = isProtected;
            IsVerified = isVerified;
            IsTranslator = isTranslator;
            IsContributorsEnabled = isContributorsEnabled;
            IsGeoEnabled = isGeoEnabled;
            StatusesCount = statusesCount;
            FollowingsCount = followingsCount;
            FollowersCount = followersCount;
            FavoritesCount = favoritesCount;
            ListedCount = listedCount;
            Language = language;
            CreatedAt = createdAt;
            UrlEntities = urlEntities ?? throw new ArgumentNullException(nameof(urlEntities));
            DescriptionEntities = descriptionEntities ?? throw new ArgumentNullException(nameof(descriptionEntities));
        }

        public TwitterUser(long id, string screenName, Uri profileImageUri)
        {
            Id = id;
            ScreenName = screenName;
            ProfileImageUri = profileImageUri;
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
        [NotNull]
        public TwitterUrlEntity[] UrlEntities { get; }

        /// <summary>
        /// Entities of user description
        /// </summary>
        [NotNull]
        public TwitterEntity[] DescriptionEntities { get; }

        [NotNull]
        public string UserPermalink => String.Format(TwitterUserUrl, ScreenName);

        [NotNull]
        public string FavstarUserPermalink => String.Format(FavstarUserUrl, ScreenName);

        [NotNull]
        public string TwilogUserPermalink => String.Format(TwilogUserUrl, ScreenName);

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Id == ((TwitterUser)obj).Id;
        }

        [NotNull]
        public string GetEntityAidedUrl(EntityDisplayMode displayMode = EntityDisplayMode.DisplayText)
        {
            return TextEntityResolver.GetEntityAidedText(Url ?? String.Empty,
                UrlEntities, displayMode);
        }

        [NotNull]
        public string GetEntityAidedDescription(EntityDisplayMode displayMode = EntityDisplayMode.DisplayText)
        {
            return TextEntityResolver.GetEntityAidedText(Description ?? String.Empty,
                DescriptionEntities, displayMode);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}