using System;
using System.Collections;
using System.Collections.Generic;

public class ReadOnlySetView<T> : ISet<T>
{
    private ISet<T> _view;
    public ReadOnlySetView(ISet<T> toView) => _view = toView;

    public int Count => _view.Count;

    public bool IsReadOnly => _view.IsReadOnly;

    public bool Contains(T item)
    {
        return _view.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _view.CopyTo(array, arrayIndex);
    }


    public IEnumerator<T> GetEnumerator()
    {
        return _view.GetEnumerator();
    }


    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        return _view.IsProperSubsetOf(other);
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        return _view.IsProperSupersetOf(other);
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        return _view.IsSubsetOf(other);
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        return _view.IsSupersetOf(other);
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        return _view.Overlaps(other);
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return _view.SetEquals(other);
    }


    void ICollection<T>.Add(T item)
    {
        ((ICollection<T>)_view).Add(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_view).GetEnumerator();
    }

    public bool Remove(T item) => throw new NotSupportedException($"Cannot modify read only set");
    public void SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException($"Cannot modify read only set");
    public void UnionWith(IEnumerable<T> other) => throw new NotSupportedException($"Cannot modify read only set");
    public void IntersectWith(IEnumerable<T> other) => throw new NotSupportedException($"Cannot modify read only set");
    public bool Add(T item) => throw new NotSupportedException($"Cannot modify read only set");
    public void Clear() => throw new NotSupportedException($"Cannot modify read only set");
    public void ExceptWith(IEnumerable<T> other) => throw new NotSupportedException($"Cannot modify read only set");
}