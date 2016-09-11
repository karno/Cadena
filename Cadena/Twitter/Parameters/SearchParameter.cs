using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Cadena.Twitter.Parameters
{
    public sealed class SearchParameter : ParameterBase
    {
        private string _query;

        public SearchResultType ResultType { get; }

        [CanBeNull]
        public string GeoCode { get; }

        [CanBeNull]
        public string Lang { get; }

        [CanBeNull]
        public string Locale { get; }

        public int? Count { get; }

        public DateTime? UntilDate { get; }

        public long? SinceId { get; }

        public long? MaxId { get; }

        public SearchParameter([NotNull] string query, SearchResultType resultType = SearchResultType.Mixed,
           [CanBeNull] string geoCode = null, [CanBeNull] string lang = null, [CanBeNull] string locale = null,
           int? count = null, DateTime? untilDate = null, long? sinceId = null, long? maxId = null)
        {
            _query = query;
            ResultType = resultType;
            GeoCode = geoCode;
            Lang = lang;
            Locale = locale;
            Count = count;
            UntilDate = untilDate;
            SinceId = sinceId;
            MaxId = maxId;
            if (query == null) throw new ArgumentNullException(nameof(query));
        }

        [NotNull]
        public string Query
        {
            get { return _query; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                _query = value;
            }
        }

        public override void SetDictionary(IDictionary<string, object> target)
        {
            target["q"] = _query;
            target["geocode"] = GeoCode;
            target["lang"] = Lang;
            target["locale"] = Locale;
            target["result_type"] = ResultType.ToString().ToLower();
            target["count"] = Count;
            target["until"] = UntilDate?.ToString("yyyy-MM-dd");
            target["since_id"] = SinceId;
            target["max_id"] = MaxId;
        }
    }


    public enum SearchResultType
    {
        Mixed,
        Recent,
        Popular,
    }
}
