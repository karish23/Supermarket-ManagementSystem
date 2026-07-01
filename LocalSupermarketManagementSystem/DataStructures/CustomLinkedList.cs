namespace LocalSupermarketManagementSystem.DataStructures;

public class CustomLinkedList<T>
{
    private sealed class Node
    {
        public T Value { get; }
        public Node? Next { get; set; }

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node? _head;
    private Node? _tail;

    public int Count { get; private set; }

    public void AddLast(T item)
    {
        var node = new Node(item);
        if (_head == null)
        {
            _head = node;
            _tail = node;
        }
        else
        {
            _tail!.Next = node;
            _tail = node;
        }

        Count++;
    }

    public IEnumerable<T> AsEnumerable()
    {
        var current = _head;
        while (current != null)
        {
            yield return current.Value;
            current = current.Next;
        }
    }

    public CustomLinkedList<T> FindAll(Func<T, bool> predicate)
    {
        var result = new CustomLinkedList<T>();
        var current = _head;
        while (current != null)
        {
            if (predicate(current.Value))
            {
                result.AddLast(current.Value);
            }
            current = current.Next;
        }
        return result;
    }

    public T? FirstOrDefault(Func<T, bool> predicate)
    {
        var current = _head;
        while (current != null)
        {
            if (predicate(current.Value))
            {
                return current.Value;
            }
            current = current.Next;
        }
        return default;
    }

    public List<T> ToList()
    {
        var list = new List<T>();
        foreach (var item in AsEnumerable())
        {
            list.Add(item);
        }
        return list;
    }
}
