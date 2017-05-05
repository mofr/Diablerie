using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int sizeX;
    public int sizeY;
    Entry[] grid;
    static List<Entry> coveredEntries = new List<Entry>();

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
        if (entry.item != null)
        {
            Remove(entry);

            if (OnUpdate != null)
                OnUpdate();
        }
        return entry.item;
    }

    public bool Fit(Item item, int x0, int y0, out List<Entry> covered)
    {
        coveredEntries.Clear();
        covered = coveredEntries;

        if (x0 < 0 || y0 < 0 || 
            x0 + item.info.invWidth > sizeX || y0 + item.info.invHeight > sizeY)
            return false;
        if (item.info.invHeight <= 0 || item.info.invWidth <= 0)
            return false;

        int i = y0 * sizeX + x0;
        for(int y = 0; y < item.info.invHeight; ++y)
        {
            for(int x = 0; x < item.info.invWidth; ++x)
            {
                var entry = grid[i + x];
                if (entry.item != null && !covered.Contains(entry))
                {
                    covered.Add(entry);
                }
            }
            i += sizeX;
        }
        return true;
    }

    private void Remove(Entry entry)
    {
        Item item = entry.item;
        entry.item = null;
        Fill(entry, item.info.invWidth, item.info.invHeight);
        entries.RemoveAll(e => e.item == item);
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

    public bool Put(Item item, int x0, int y0, out Item poppedItem, int maxPops = 1)
    {
        poppedItem = null;
        if (!Fit(item, x0, y0, out coveredEntries))
            return false;

        if (coveredEntries.Count > maxPops)
            return false;

        if (coveredEntries.Count > 0)
        {
            poppedItem = coveredEntries[0].item;
            Remove(coveredEntries[0]);
        }

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
        Item popped;
        for (int x = 0; x < sizeX; ++x)
        {
            for (int y = 0; y < sizeY; ++y)
            {
                if (Put(item, x, y, out popped, maxPops: 0))
                    return true;
            }
        }

        return false;
    }
}
