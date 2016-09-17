using System;
using System.Collections;
using System.Collections.Generic;

namespace Cadena.Meteor._Internals
{
    public class ListDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly int _capacity;
        readonly TKey[] _keys;
        readonly TValue[] _values;

        private int _count;

        public ListDictionary(int capacity)
        {
            _capacity = capacity;
            _keys = new TKey[capacity];
            _values = new TValue[capacity];
            _count = 0;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new KeyValuePair<TKey, TValue>(_keys[i], _values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            for (var i = 0; i < _count; i++)
            {
                if (Equals(_keys[i], item.Key) && Equals(_values[i], item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public int Count => _count;
        public bool IsReadOnly => false;
        public bool ContainsKey(TKey key)
        {
            for (var i = 0; i < _count; i++)
            {
                if (Equals(_keys[i], key))
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            this[key] = value;
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            for (var i = 0; i < _count; i++)
            {
                if (Equals(_keys[i], key))
                {
                    value = _values[i];
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get
            {
                for (var i = 0; i < _count; i++)
                {
                    if (Equals(_keys[i], key))
                    {
                        return _values[i];
                    }
                }
                return default(TValue);
            }
            set
            {
                for (var i = 0; i < _count; i++)
                {
                    if (Equals(_keys[i], key))
                    {
                        _values[i] = value;
                    }
                }
                if (_count < _capacity)
                {
                    _keys[_count] = key;
                    _values[_count] = value;
                    _count++;
                }
                else
                {
                    throw new ArgumentException("over the specified capacity.");
                }
            }
        }

        public ICollection<TKey> Keys => _keys;

        public ICollection<TValue> Values => _values;

        public IDictionary<TKey, TValue> CreateDictionary()
        {
            return new Dictionary<TKey, TValue>(this);
        }
    }
}
