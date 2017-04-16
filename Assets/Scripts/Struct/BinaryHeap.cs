using System;

public class BinaryHeap<T> where T: IComparable<T>
{
    T[] items;
    int count = 0;
    int maxSize;

    public BinaryHeap(int maxSize)
    {
        // zero element is unused for the convinience of the implementation
        items = new T[maxSize + 1];
        this.maxSize = maxSize;
    }

    public void Add(T item)
    {
        ++count;
        int index = count;
        items[index] = item;

        int parent = index / 2;
        while (index > 1 && items[parent].CompareTo(items[index]) > 0)
        {
            Swap(ref items[parent], ref items[index]);
            index = parent;
            parent = index / 2;
        }
    }

    public T Take()
    {
        T result = items[1];
        items[1] = items[count];
        --count;

        int iterCount = 0;

        int index = 1;
        int child = index * 2;
        while (child <= count)
        {
            if (iterCount++ > 1000)
                throw new Exception("too long");

            int left = child;
            int right = child + 1;
            int smallest = index;
            if (items[smallest].CompareTo(items[left]) > 0)
                smallest = left;
            if (right <= count && items[smallest].CompareTo(items[right]) > 0)
                smallest = right;
            if (index != smallest)
            {
                Swap(ref items[index], ref items[smallest]);
                index = smallest;
                child = index * 2;
            }
            else
            {
                break;
            }
        }

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

    public int MaxSize
    {
        get
        {
            return maxSize;
        }
    }

    public override string ToString()
    {
        if (count == 0)
            return "";

        string result = items[1].ToString();

        for(int i = 2; i < count + 1; ++i)
        {
            result += ", " + items[i];
        }
        
        return "[" + result + "]";
    }

    static void Swap(ref T a, ref T b)
    {
        T tmp = a;
        a = b;
        b = tmp;
    }
}
