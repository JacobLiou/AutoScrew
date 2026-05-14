namespace AutoScrew.Domain.Curves;

/// <summary>
/// Fixed-capacity ring buffer for streaming curve samples (Design §6.2).
/// </summary>
public sealed class RingBuffer<T>
{
    private readonly T[] _items;
    private int _head;
    private int _count;

    public RingBuffer(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        Capacity = capacity;
        _items = new T[capacity];
    }

    public int Capacity { get; }

    public int Count => _count;

    public void Push(in T item)
    {
        if (_count < Capacity)
        {
            var index = (_head + _count) % Capacity;
            _items[index] = item;
            _count++;
        }
        else
        {
            _items[_head] = item;
            _head = (_head + 1) % Capacity;
        }
    }

    public void CopyTo(Span<T> destination)
    {
        if (destination.Length < _count)
            throw new ArgumentException("Destination span too small.", nameof(destination));

        if (_count == 0)
            return;

        if (_count < Capacity)
        {
            for (var i = 0; i < _count; i++)
                destination[i] = _items[i];
        }
        else
        {
            for (var i = 0; i < _count; i++)
                destination[i] = _items[(_head + i) % Capacity];
        }
    }

    public T[] ToArray()
    {
        var arr = new T[_count];
        CopyTo(arr.AsSpan());
        return arr;
    }
}
