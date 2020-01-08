using UnityEngine;

namespace Diablerie.Engine.Utility
{
    public class SortingLayers
    {
        public static readonly int LowerWall = SortingLayer.NameToID("LowerWall");
        public static readonly int Floor = SortingLayer.NameToID("Floor");
        public static readonly int Shadow = SortingLayer.NameToID("Shadow");
        public static readonly int Roof = SortingLayer.NameToID("Roof");
        public static readonly int WorldUI = SortingLayer.NameToID("WorldUI");
        public static readonly int UI = SortingLayer.NameToID("UI");
    }
}