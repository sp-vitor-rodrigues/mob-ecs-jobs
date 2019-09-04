using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public static class NativeListExtensions
{
    /// <summary>
    /// Remove an element from a <see cref="NativeList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of NativeList.</typeparam>
    /// <typeparam name="TI">The type of element.</typeparam>
    /// <param name="list">The NativeList.</param>
    /// <param name="element">The element.</param>
    /// <returns>True if removed, else false.</returns>
    public static bool Remove<T, TI>(this NativeList<T> list, TI element)
        where T : struct, IEquatable<TI>
        where TI : struct
    {
        var index = list.IndexOf(element);
        if (index < 0)
        {
            return false;
        }

        list.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Remove an element from a <see cref="NativeList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="list">The list to remove from.</param>
    /// <param name="index">The index to remove.</param>
    public static void RemoveAt<T>(this NativeList<T> list, int index)
        where T : struct
    {
        list.RemoveRange(index, 1);
    }

    /// <summary>
    /// Removes a range of elements from a <see cref="NativeList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="list">The list to remove from.</param>
    /// <param name="index">The index to remove.</param>
    /// <param name="count">Number of elements to remove.</param>
    public static unsafe void RemoveRange<T>(this NativeList<T> list, int index, int count)
        where T : struct
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        if ((uint)index >= (uint)list.Length)
        {
            throw new IndexOutOfRangeException(
                $"Index {index} is out of range in NativeList of '{list.Length}' Length.");
        }
#endif

        int elemSize = UnsafeUtility.SizeOf<T>();
        byte* basePtr = (byte*)list.GetUnsafePtr();

        UnsafeUtility.MemMove(basePtr + (index * elemSize), basePtr + ((index + count) * elemSize), elemSize * (list.Length - count - index));

        // No easy way to change length so we just loop this unfortunately.
        for (var i = 0; i < count; i++)
        {
            list.RemoveAtSwapBack(list.Length - 1);
        }
    }
}
