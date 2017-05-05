using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    Entry[] grid;

    public delegate void OnUpdateHandler();
    public event OnUpdateHandler OnUpdate;

    static public Inventory Create(GameObject gameObject, int sizeX, int sizeY)
    {
        var inventory = gameObject.AddComponent<Inventory>();
        inventory.sizeX = sizeX;
        inventory.sizeY = sizeY;
        inventory.grid = new Entry[sizeX * sizeY];
        return inventory;
    }

    List<Entry> _entries = new List<Entry>();

    public struct Entry
    {
        public Item item;
        public int x;
        public int y;
    }

    public List<Entry> entries
    {
        get
        {
            return _entries;
        }
    }

    public Entry At(int x, int y)
    {
        if (x < 0 || y < 0 || x >= sizeX || y >= sizeY)
            return new Entry() { item = null };
        return grid[y * sizeX + x];
    }

    public Item Take(int x, int y)
    {
        Entry entry = At(x, y);
        Item item = entry.item;
        if (item != null)
        {
            entry.item = null;
            Fill(entry, item.info.invWidth, item.info.invHeight);
            entries.RemoveAll(e => e.item == item);

            if (OnUpdate != null)
                OnUpdate();
        }
        return item;
    }

    public bool Fits(Item item, int x0, int y0)
    {
        if (x0 < 0 || y0 < 0 || x0 >= sizeX || y0 >= sizeY)
            return false;
        if (item.info.invHeight <= 0 || item.info.invWidth <= 0)
            return false;

        int i = y0 * sizeX + x0;
        for(int y = 0; y < item.info.invHeight; ++y)
        {
            for(int x = 0; x < item.info.invWidth; ++x)
            {
                if (i + x >= grid.Length)
                    return false;
                if (grid[i + x].item != null)
                    return false;
            }
            i += sizeX;
        }
        return true;
    }

    private void Fill(Entry entry, int width, int height)
    {
        int i = entry.y * sizeX + entry.x;
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                grid[i + x] = entry;
            }
            i += sizeX;
        }
    }

    public bool Put(Item item, int x0, int y0)
    {
        if (!Fits(item, x0, y0))
            return false;

        var entry = new Entry();
        entry.item = item;
        entry.x = x0;
        entry.y = y0;
        _entries.Add(entry);
        Fill(entry, item.info.invWidth, item.info.invHeight);

        if (OnUpdate != null)
            OnUpdate();

        return true;
    }

    public bool Put(Item item)
    {
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                if (Put(item, x, y))
                    return true;
            }
        }

        return false;
    }
}
