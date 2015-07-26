using System.Collections.Generic;

namespace Cadena.Api.Parameters
{
    public sealed class UserParameter : ParameterBase
    {
        public long? UserId { get; }

        public string ScreenName { get; }

        internal string UserIdKey { get; set; } = "user_id";

        internal string ScreenNameKey { get; set; } = "screen_name";

        internal void SetKeyAsSource()
        {
            UserIdKey = "source_id";
            ScreenNameKey = "source_screen_name";
        }

        internal void SetKeyAsTarget()
        {
            UserIdKey = "target_id";
            ScreenNameKey = "target_screen_name";
        }

        public UserParameter(long userId)
        {
            UserId = userId;
            ScreenName = null;
        }

        public UserParameter(string screenName)
        {
            UserId = null;
            ScreenName = screenName;
        }

        public override void SetDictionary(IDictionary<string, object> target)
        {
            target[UserIdKey] = UserId;
            target[ScreenNameKey] = ScreenName;
        }
    }
}
