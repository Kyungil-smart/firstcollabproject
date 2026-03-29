using System;
using System.Collections.Generic;
using System.Linq;

public delegate T SelectionStrategy<T>(IEnumerable<T> items);
/// <summary>
/// 다양한 타입의 객체를 등록하고 관리하는 C# 클래스
/// </summary>
public static class Registry<T> where T : class
{
    static readonly HashSet<T> _items = new();
    public static int Count => _items.Count;
    public static event Action<T> OnRemoved;

    public static bool TryAdd(T item)
    {
        return item != null && _items.Add(item);
    }

    public static void Remove(T item)
    {
        if (_items.Remove(item)) OnRemoved?.Invoke(item);
    }
    public static void Clear()
    {
        _items.Clear();
    }

    public static T GetFirst()
    {
        return _items.FirstOrDefault();
    }

    public static T Get(SelectionStrategy<T> strategy) => strategy(_items);

    public static IEnumerable<T> All => _items;

}
