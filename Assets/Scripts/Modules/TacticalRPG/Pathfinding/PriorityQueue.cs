using System;
using System.Collections.Generic;

/// <summary>
/// A generic min-priority queue based on a binary heap.
/// Supports fast insertions, deletions, and updates.
/// Requires items to implement <see cref="IComparable{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements stored in the queue.</typeparam>
public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> heap = new List<T>();
    private Dictionary<T, int> indexMap = new Dictionary<T, int>(); // Maps item to its index in the heap

    /// <summary>
    /// Gets the number of elements in the queue.
    /// </summary>
    public int Count => heap.Count;

    /// <summary>
    /// Adds an item to the queue.
    /// </summary>
    /// <param name="item">The item to enqueue.</param>
    public void Enqueue(T item)
    {
        heap.Add(item);
        int index = heap.Count - 1;
        indexMap[item] = index;
        HeapifyUp(index); // Restore heap property upwards
    }

    /// <summary>
    /// Removes and returns the item with the highest priority (minimum value).
    /// </summary>
    /// <returns>The item with the highest priority.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the queue is empty.</exception>
    public T Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T root = heap[0];
        T last = heap[heap.Count - 1];

        heap[0] = last;
        indexMap[last] = 0;

        heap.RemoveAt(heap.Count - 1);
        indexMap.Remove(root);

        if (heap.Count > 0)
            HeapifyDown(0); // Restore heap property downwards

        return root;
    }

    /// <summary>
    /// Inserts a new item or updates an existing item if already present.
    /// Adjusts its position in the heap to maintain priority order.
    /// </summary>
    /// <param name="item">The item to enqueue or update.</param>
    public void EnqueueOrUpdate(T item)
    {
        if (indexMap.TryGetValue(item, out int index))
        {
            heap[index] = item;
            HeapifyUp(index);
            HeapifyDown(index);
        }
        else
        {
            Enqueue(item);
        }
    }

    /// <summary>
    /// Checks if the queue contains a specific item.
    /// </summary>
    /// <param name="item">The item to check for.</param>
    /// <returns>True if the item exists in the queue; otherwise, false.</returns>
    public bool Contains(T item) => indexMap.ContainsKey(item);

    /// <summary>
    /// Restores the heap property by moving an item up.
    /// </summary>
    /// <param name="index">Index of the item to move.</param>
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].CompareTo(heap[parent]) >= 0)
                break;

            Swap(index, parent);
            index = parent;
        }
    }

    /// <summary>
    /// Restores the heap property by moving an item down.
    /// </summary>
    /// <param name="index">Index of the item to move.</param>
    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].CompareTo(heap[smallest]) < 0)
                smallest = left;
            if (right <= lastIndex && heap[right].CompareTo(heap[smallest]) < 0)
                smallest = right;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    /// <summary>
    /// Swaps two items in the heap and updates their indices in the map.
    /// </summary>
    /// <param name="i">First index.</param>
    /// <param name="j">Second index.</param>
    private void Swap(int i, int j)
    {
        T temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;

        indexMap[heap[i]] = i;
        indexMap[heap[j]] = j;
    }
}