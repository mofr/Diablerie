using UnityEngine;
using System.Collections.Generic;

public class Maze
{
    public struct Palette
    {
        public LevelPreset[] rooms;
        public LevelPreset[] themedRooms;
        public LevelPreset[][] special;
    }

    struct Room
    {
        public bool fill;
        public bool special;
        public int specialIndex;
        public int dirMask;
    }
    
    static Room[,] grid = new Room[8, 8];
    static Vector2i[] dirs = new Vector2i[] {
        new Vector2i(-1, 0),
        new Vector2i(1, 0),
        new Vector2i(0, 1),
        new Vector2i(0, -1),
    };

    public static void Generate(LevelBuilder builder, Palette palette)
    {
        Clear();
        GenerateRooms(2);
        GenerateSpecialRooms(palette);
        Build(builder, palette);
    }

    private static void Clear()
    {
        for (int x = 0; x < grid.GetLength(0); ++x)
        {
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                grid[x, y] = new Room()
                {
                    fill = false,
                    special = false,
                    specialIndex = -1,
                    dirMask = 0
                };
            }
        }
    }

    private static void GenerateRooms(int roomCount)
    {
        Vector2i c = new Vector2i(3, 3);
        for (int i = 0; i < roomCount;)
        {
            int dirIndex = Random.Range(0, 4);
            Vector2i newPos = c + dirs[dirIndex];
            while (!Empty(newPos.x, newPos.y))
            {
                ++dirIndex;
                newPos = c + dirs[dirIndex % 4];
                if (dirIndex > 8)
                {
                    Debug.LogWarning("Failed to find an empty place for a room");
                    return;
                }
            }

            c = newPos;
            grid[c.x, c.y].fill = true;
            grid[c.x, c.y].dirMask = 0xf;
            ++i;
        }
    }

    private static void GenerateSpecialRooms(Palette palette)
    {
        for (int specialIndex = 0; specialIndex < palette.special.Length; ++specialIndex)
        {
            Vector2i pos;
            int dirMask = 0;
            do
            {
                pos = new Vector2i(
                    Random.Range(0, grid.GetLength(0)),
                    Random.Range(0, grid.GetLength(1)));
                if (grid[pos.x, pos.y].fill)
                    continue;
                dirMask = GenerateDirMask(pos.x, pos.y);
            }
            while (dirMask == 0);

            int dirIndex = Random.Range(0, 4);
            while ((dirMask & (1 << dirIndex)) == 0)
                dirIndex = (dirIndex + 1) % 4;
            dirMask = 1 << dirIndex;
            grid[pos.x, pos.y] = new Room()
            {
                fill = true,
                special = true,
                specialIndex = specialIndex,
                dirMask = dirMask
            };
        }
    }

    private static void Build(LevelBuilder builder, Palette palette)
    {
        for (int x = 0; x < grid.GetLength(0); ++x)
        {
            for (int y = 0; y < grid.GetLength(1); ++y)
            {
                PlaceRoom(builder, palette, x, y);
            }
        }
    }

    private static void PlaceRoom(LevelBuilder builder, Palette palette, int x, int y)
    {
        Room room = grid[x, y];
        if (!room.fill)
            return;

        if (room.special)
        {
            int index = SpecialIndex(room.dirMask);
            builder.Place(palette.special[room.specialIndex][index], new Vector2i(x, y));
        }
        else
        {
            int index = GenerateDirMask(x, y);
            LevelPreset[] set = Random.Range(0, 2) == 0 ? palette.rooms : palette.themedRooms;
            builder.Place(set[index], new Vector2i(x, y));
        }
    }

    private static int GenerateDirMask(int x, int y)
    {
        int index = 0;
        index |= HasConnection(1, x - 1, y);
        index |= HasConnection(0, x + 1, y) << 1;
        index |= HasConnection(3, x, y + 1) << 2;
        index |= HasConnection(2, x, y - 1) << 3;
        return index;
    }

    private static int HasConnection(int dirIndex, int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return 0;

        if ((grid[x, y].dirMask & (1 << dirIndex)) == 0)
            return 0;

        return 1;
    }

    private static bool Empty(int x, int y)
    {
        if (x < 0 || x >= grid.GetLength(0) || y < 0 || y >= grid.GetLength(1))
            return false;

        return !grid[x, y].fill;
    }

    private static int SpecialIndex(int dirMask)
    {
        switch (dirMask)
        {
            case 1: return 0;
            case 2: return 1;
            case 4: return 2;
            case 8: return 3;
            default: return -1;
        }
    }

    public static T RandomChoice<T>(List<T> array)
    {
        return array[Random.Range(0, array.Count)];
    }
}