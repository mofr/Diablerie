using System;

public class BinaryHeap<T> where T: IComparable<T>
{
    T[] items;
    int count = 0;

    public BinaryHeap(int size)
    {
        items = new T[size];
    }

    public void Add(T item)
    {
        int index = count;
        items[index] = item;
        ++count;

        int parent = (index - 1) / 2;
        while (index != parent)
        {
            if (items[parent].CompareTo(items[index]) > 0)
            {
                Swap(ref items[parent], ref items[index]);
                index = parent;
                parent = (index - 1) / 2;
            }
            else
            {
                break;
            }
        }
    }

    public T Take()
    {
        T result = items[0];
        items[0] = items[count - 1];
        --count;

        return result;
    }

    public void Clear()
    {
        count = 0;
    }

    public int Count
    {
        get
        {
            return count;
        }
    }

    public override string ToString()
    {
        if (count == 0)
            return "";

        string result = items[0].ToString();

        for(int i = 1; i < count; ++i)
        {
            result += ", " + items[i];
        }
        
        return "[" + result + "]";
    }

    void Heapify(int index)
    {

    }

    static void Swap(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }
}
