using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlistCS
{
    public class DictionaryOrdered<t1, t2> : IDictionary<t1, t2>,IList<KeyValuePair<t1,t2>>
    {
        private List<KeyValuePair<t1, t2>> _Content = new List<KeyValuePair<t1, t2>>();

        public t2 this[t1 key]
        {
            get
            {
                t2 result;
                if (!TryGetValue(key,out result))
                {
                    throw new KeyNotFoundException();
                }
                return result;

            }

            set
            {
                var index = IndexOf(key);
                if (index < 0)
                {
                    Add(new KeyValuePair<t1, t2>(key, value));
                }
                else
                {
                    _Content[index] = new KeyValuePair<t1, t2>(key, value);
                }
            }
        }

        public KeyValuePair<t1, t2> this[int index]
        {
            get
            {
                return ((IList<KeyValuePair<t1, t2>>)_Content)[index];
            }

            set
            {
                ((IList<KeyValuePair<t1, t2>>)_Content)[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IList<KeyValuePair<t1, t2>>)_Content).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IList<KeyValuePair<t1, t2>>)_Content).IsReadOnly;
            }
        }

        public ICollection<t1> Keys
        {
            get
            {
                var result = new t1[_Content.Count];
                for(int i = 0; i < _Content.Count; i++)
                {
                    result[i] = _Content[i].Key;
                }
                return result;
            }
        }

        public ICollection<t2> Values
        {
            get
            {
                var result = new t2[_Content.Count];
                for (int i = 0; i < _Content.Count; i++)
                {
                    result[i] = _Content[i].Value;
                }
                return result;
            }
        }

        public void Add(KeyValuePair<t1, t2> item)
        {
            Add(item.Key, item.Value);
        }

        public void Add(t1 key, t2 value)
        {
            var index=IndexOf(key);
            if (index < 0)
            {
                _Content.Add(new KeyValuePair<t1, t2>(key, value));
            }
            else
            {
                throw new System.ArgumentException("");
            }
        }

        public void Clear()
        {
            ((IList<KeyValuePair<t1, t2>>)_Content).Clear();
        }

        public bool Contains(KeyValuePair<t1, t2> item)
        {
            return ((IList<KeyValuePair<t1, t2>>)_Content).Contains(item);
        }

        public bool ContainsKey(t1 key)
        {
            t2 dummy;
            return TryGetValue(key, out dummy);
        }

        public void CopyTo(KeyValuePair<t1, t2>[] array, int arrayIndex)
        {
            ((IList<KeyValuePair<t1, t2>>)_Content).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<t1, t2>> GetEnumerator()
        {
            return ((IList<KeyValuePair<t1, t2>>)_Content).GetEnumerator();
        }

        public int IndexOf(KeyValuePair<t1, t2> item)
        {
            return ((IList<KeyValuePair<t1, t2>>)_Content).IndexOf(item);
        }

        public int IndexOf(t1 key)
        {
            for (int i = 0; i < _Content.Count; i++)
            {
                if (_Content[i].Key.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, KeyValuePair<t1, t2> item)
        {
            ((IList<KeyValuePair<t1, t2>>)_Content).Insert(index, item);
        }

        public bool Remove(t1 key)
        {
            var index = IndexOf(key);
            if (index >= 0)
            {
                _Content.RemoveAt(index);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool Remove(KeyValuePair<t1, t2> item)
        {
            return ((IList<KeyValuePair<t1, t2>>)_Content).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<KeyValuePair<t1, t2>>)_Content).RemoveAt(index);
        }

        public bool TryGetValue(t1 key, out t2 value)
        {
            for (int i = 0; i < _Content.Count; i++)
            {
                if (key.Equals(_Content[i].Key))
                {
                    value = _Content[i].Value;
                    return true;
                }
            }
            value = default(t2);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<KeyValuePair<t1, t2>>)_Content).GetEnumerator();
        }
    }
}
