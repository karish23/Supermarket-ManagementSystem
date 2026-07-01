namespace LocalSupermarketManagementSystem.DataStructures;

public class CustomHashTable<TKey, TValue> where TKey : notnull
{
    private sealed class Entry
    {
        public TKey Key { get; }
        public TValue Value { get; set; }
        public Entry? Next { get; set; }

        public Entry(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    private Entry?[] _buckets;

    public int Count { get; private set; }

    public CustomHashTable(int capacity = 101)
    {
        _buckets = new Entry?[capacity];
    }

    public void AddOrUpdate(TKey key, TValue value)
    {
        if ((double)(Count + 1) / _buckets.Length > 0.75)
        {
            Resize();
        }

        int index = GetBucketIndex(key);
        var current = _buckets[index];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                current.Value = value;
                return;
            }
            current = current.Next;
        }

        var entry = new Entry(key, value) { Next = _buckets[index] };
        _buckets[index] = entry;
        Count++;
    }

    public bool TryGetValue(TKey key, out TValue? value)
    {
        int index = GetBucketIndex(key);
        var current = _buckets[index];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                value = current.Value;
                return true;
            }
            current = current.Next;
        }

        value = default;
        return false;
    }

    public bool Remove(TKey key)
    {
        int index = GetBucketIndex(key);
        Entry? previous = null;
        var current = _buckets[index];

        while (current != null)
        {
            if (EqualityComparer<TKey>.Default.Equals(current.Key, key))
            {
                if (previous == null)
                {
                    _buckets[index] = current.Next;
                }
                else
                {
                    previous.Next = current.Next;
                }

                Count--;
                return true;
            }

            previous = current;
            current = current.Next;
        }

        return false;
    }

    private int GetBucketIndex(TKey key)
    {
        int hash = key.GetHashCode() & 0x7fffffff;
        return hash % _buckets.Length;
    }

    private void Resize()
    {
        var oldBuckets = _buckets;
        _buckets = new Entry?[oldBuckets.Length * 2 + 1];
        Count = 0;

        foreach (var bucket in oldBuckets)
        {
            var current = bucket;
            while (current != null)
            {
                AddOrUpdate(current.Key, current.Value);
                current = current.Next;
            }
        }
    }
}
