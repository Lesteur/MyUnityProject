using System;
using System.Collections.Generic;
using UnityEngine.Pool;

/// <summary>
/// A generic minimum-priority queue based on a binary heap.
/// Supports efficient insertions, deletions, and priority updates,
/// with pooled collections to minimize GC allocations.
/// </summary>
/// <typeparam name="T">The type of elements stored in the queue. Must implement <see cref="IComparable{T}"/>.</typeparam>
public class PriorityQueue<T> : IDisposable where T : IComparable<T>
{
    private readonly List<T> _heap;
    private readonly Dictionary<T, int> _indexMap;

    /// <summary>
    /// Gets the number of elements in the queue.
    /// </summary>
    public int Count => _heap.Count;

    public PriorityQueue()
    {
        _heap = ListPool<T>.Get();
        _indexMap = DictionaryPool<T, int>.Get();
    }

    /// <summary>
    /// Adds an item to the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void Enqueue(T item)
    {
        _heap.Add(item);
        int index = _heap.Count - 1;
        _indexMap[item] = index;
        HeapifyUp(index);
    }

    /// <summary>
    /// Removes and returns the item with the smallest priority value.
    /// </summary>
    /// <returns>The item with the highest priority (minimum value).</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public T Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        T root = _heap[0];
        T last = _heap[_heap.Count - 1];

        _heap[0] = last;
        _indexMap[last] = 0;

        _heap.RemoveAt(_heap.Count - 1);
        _indexMap.Remove(root);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return root;
    }

    /// <summary>
    /// Inserts a new item or updates an existing item if already present,
    /// adjusting its position in the heap to maintain priority order.
    /// </summary>
    /// <param name="item">The item to enqueue or update.</param>
    public void EnqueueOrUpdate(T item)
    {
        if (_indexMap.TryGetValue(item, out int index))
        {
            _heap[index] = item;
            HeapifyUp(index);
            HeapifyDown(index);
        }
        else
        {
            Enqueue(item);
        }
    }

    /// <summary>
    /// Checks whether the queue contains the specified item.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if the item exists in the queue; otherwise, <c>false</c>.</returns>
    public bool Contains(T item) => _indexMap.ContainsKey(item);

    /// <summary>
    /// Restores the heap property by moving an item up the binary heap.
    /// </summary>
    /// <param name="index">The index of the item to move.</param>
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (_heap[index].CompareTo(_heap[parent]) >= 0)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    /// <summary>
    /// Restores the heap property by moving an item down the binary heap.
    /// </summary>
    /// <param name="index">The index of the item to move.</param>
    private void HeapifyDown(int index)
    {
        int lastIndex = _heap.Count - 1;

        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left <= lastIndex && _heap[left].CompareTo(_heap[smallest]) < 0)
                smallest = left;

            if (right <= lastIndex && _heap[right].CompareTo(_heap[smallest]) < 0)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    /// <summary>
    /// Swaps two items in the heap and updates their indices in the index map.
    /// </summary>
    /// <param name="i">The first index.</param>
    /// <param name="j">The second index.</param>
    private void Swap(int i, int j)
    {
        T temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;

        _indexMap[_heap[i]] = i;
        _indexMap[_heap[j]] = j;
    }

    /// <summary>
    /// Clears the queue and releases pooled resources.
    /// </summary>
    public void Dispose()
    {
        ListPool<T>.Release(_heap);
        DictionaryPool<T, int>.Release(_indexMap);
    }
}