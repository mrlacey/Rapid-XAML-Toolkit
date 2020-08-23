// Copyright (c) Matt Lacey Ltd. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;

namespace RapidXamlToolkit.XamlAnalysis
{
    internal class SizeLimitedDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> dictionary;
        private readonly Queue<TKey> keys;
        private readonly int capacity;

        public SizeLimitedDictionary(int capacity)
        {
            this.keys = new Queue<TKey>(capacity);
            this.capacity = capacity;
            this.dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public TValue this[TKey key]
        {
            get { return this.dictionary[key]; }
        }

        public void Add(TKey key, TValue value)
        {
            if (this.dictionary.Count == this.capacity)
            {
                var oldestKey = this.keys.Dequeue();
                this.dictionary.Remove(oldestKey);
            }

            this.dictionary.Add(key, value);
            this.keys.Enqueue(key);
        }

        public bool ContainsKey(TKey key)
        {
            return this.dictionary.ContainsKey(key);
        }
    }
}
