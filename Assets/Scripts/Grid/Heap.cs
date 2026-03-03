using System.Collections.Generic;
using System;
using UnityEngine.PlayerLoop;


public class Heap<T> where T : IHeapItem<T>
{
    private T[] _items;
    private int _count;
     
    public int Count => _count;

    public Heap(int maxSize)
    {
        _items = new T[maxSize];
    }

    public void Clear()
    {
        _count = 0;
    }
    
    public void Add(T item)
    {
        item.HeapIndex = _count;
        _items[_count] = item;
        SortUp(item);
        _count++;
    }
    
    public T RemoveFirst()
    {
        T first = _items[0];
        _count--;

        _items[0] = _items[_count];
        _items[0].HeapIndex = 0;

        SortDown(_items[0]);

        return first;
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        return Equals(_items[item.HeapIndex], item);
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int left = item.HeapIndex * 2 + 1;
            int right = item.HeapIndex * 2 + 2;

            int swapIndex = 0;

            if (left < _count)
            {
                swapIndex = left;

                if (right < _count)
                {
                    if (_items[left].CompareTo(_items[right]) > 0)
                        swapIndex = right;
                }

                if (item.CompareTo(_items[swapIndex]) > 0)
                {
                    Swap(item, _items[swapIndex]);
                }
                else return;
            }
            else return;
        }
    }

    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (item.HeapIndex > 0)
        {
            T parent = _items[parentIndex];

            if (item.CompareTo(parent) < 0)
            {
                Swap(item, parent);
            }
            else break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T a, T b)
    {
        _items[a.HeapIndex] = b;
        _items[b.HeapIndex] = a;

        int temp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = temp;
    }
    
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
