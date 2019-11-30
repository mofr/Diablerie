using System.Collections.Generic;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Inventory : MonoBehaviour
    {
        public int gold = 0;
        public int sizeX;
        public int sizeY;
        int[] grid;
        static List<int> coveredEntries = new List<int>();

        public delegate void OnUpdateHandler();
        public event OnUpdateHandler OnUpdate;

        static public Inventory Create(GameObject gameObject, int sizeX, int sizeY)
        {
            var inventory = gameObject.AddComponent<Inventory>();
            inventory.sizeX = sizeX;
            inventory.sizeY = sizeY;
            inventory.grid = new int[sizeX * sizeY];
            for(int i = 0; i < inventory.grid.Length; ++i)
            {
                inventory.grid[i] = -1;
            }
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

        public int At(int x, int y)
        {
            if (x < 0 || y < 0 || x >= sizeX || y >= sizeY)
                return -1;
            return grid[y * sizeX + x];
        }

        public Item ItemAt(int x, int y)
        {
            int index = At(x, y);
            if (index == -1)
                return null;

            Entry entry = _entries[index];
            return entry.item;
        }

        public Item Take(int x, int y)
        {
            int index = At(x, y);
            if (index == -1)
                return null;

            Entry entry = _entries[index];
            Remove(index);

            if (OnUpdate != null)
                OnUpdate();

            return entry.item;
        }

        public bool Fit(Item item, int x0, int y0, out List<int> covered)
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
                    var index = grid[i + x];
                    if (index != -1 && !covered.Contains(index))
                    {
                        covered.Add(index);
                    }
                }
                i += sizeX;
            }
            return true;
        }

        private void Remove(int index)
        {
            Entry entry = _entries[index];
            Fill(-1, entry.x, entry.y, entry.item.info.invWidth, entry.item.info.invHeight);
            _entries.RemoveAt(index);
            for(int i = 0; i < grid.Length; ++i)
            {
                if (grid[i] > index)
                    grid[i] -= 1;
            }
        }

        private void Fill(int index, int x0, int y0, int width, int height)
        {
            int i = y0 * sizeX + x0;
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    grid[i + x] = index;
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
                var poppedEntry = _entries[coveredEntries[0]];
                poppedItem = poppedEntry.item;
                Remove(coveredEntries[0]);
            }

            var entry = new Entry();
            entry.item = item;
            entry.x = x0;
            entry.y = y0;
            int index = _entries.Count;
            _entries.Add(entry);
            Fill(index, x0, y0, item.info.invWidth, item.info.invHeight);

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
}
