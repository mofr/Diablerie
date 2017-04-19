using UnityEngine;
using System.Collections.Generic;

public class Maze
{
    public struct Palette
    {
        public LevelPreset[] rooms;
        public LevelPreset[] previous;
        public LevelPreset[] next;
        public LevelPreset[] down;
    }

    public static void Generate(LevelBuilder builder, Palette palette)
    {
        builder.Place(palette.previous[1], new Vector2i(0, 0));
        builder.Place(palette.rooms[2], new Vector2i(1, 0));
        builder.Place(palette.rooms[0], new Vector2i(2, 0));
    }

    public static T RandomChoice<T>(List<T> array)
    {
        return array[Random.Range(0, array.Count)];
    }
}