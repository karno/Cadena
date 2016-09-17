using System.Collections.Generic;

namespace Cadena.Api
{
    public abstract class ParameterBase
    {
        public abstract void SetDictionary(IDictionary<string, object> target);

        public IDictionary<string, object> ToDictionary()
        {
            var dict = new Dictionary<string, object>();
            SetDictionary(dict);
            return dict;
        }
    }
}
