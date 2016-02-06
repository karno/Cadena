using System;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public abstract class JsonValue
    {
        public virtual bool IsObject => false;

        public virtual bool IsString => false;

        public virtual bool IsNumber => false;

        public virtual bool IsArray => false;

        public virtual bool IsBoolean => false;

        public virtual bool IsNull => false;

        public virtual int Count
        {
            get { throw new NotSupportedException(); }
        }

        [NotNull]
        public JsonValue this[int index]
        {
            get { return GetValue(index); }
        }

        [NotNull]
        public JsonValue this[string key]
        {
            get { return GetValue(key); }
        }

        public virtual bool ContainsKey(string key)
        {
            return false;
        }

        public virtual bool TryGetValue(string key, out JsonValue value)
        {
            value = null;
            return false;
        }

        [NotNull]
        public virtual JsonValue GetValue(int index)
        {
            throw new NotSupportedException();
        }

        [NotNull]
        public virtual JsonValue GetValue([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            throw new NotSupportedException();
        }

        [NotNull]
        public virtual string GetString()
        {
            throw new NotSupportedException();
        }

        public virtual long GetLong()
        {
            throw new NotSupportedException();
        }

        public virtual double GetDouble()
        {
            throw new NotSupportedException();
        }

        public virtual bool GetBoolean()
        {
            throw new NotSupportedException();
        }

        [CanBeNull]
        public JsonArray GetArray()
        {
            return this as JsonArray;
        }

        [CanBeNull]
        public JsonObject GetObject()
        {
            return this as JsonObject;
        }
    }
}

