using System;
using JetBrains.Annotations;

namespace Cadena.Meteor
{
    public abstract class JsonValue
    {
        public virtual bool IsObject { get; } = false;

        public virtual bool IsString { get; } = false;

        public virtual bool IsNumber { get; } = false;

        public virtual bool IsArray { get; } = false;

        public virtual bool IsBoolean { get; } = false;

        public virtual bool IsNull { get; } = false;

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

        public virtual bool ContainsKey([NotNull] string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return false;
        }

        public virtual bool TryGetValue([NotNull] string key, out JsonValue value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
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
            return JsonNull.Null;
        }

        [CanBeNull]
        public virtual string AsString()
        {
            return null;
        }

        public virtual long AsLong()
        {
            return default(long);
        }

        public virtual double AsDouble()
        {
            return default(double);
        }

        public virtual bool AsBoolean()
        {
            return default(bool);
        }

        [CanBeNull]
        public JsonArray AsArray()
        {
            return this as JsonArray;
        }

        [CanBeNull]
        public JsonObject AsObject()
        {
            return this as JsonObject;
        }
    }
}
