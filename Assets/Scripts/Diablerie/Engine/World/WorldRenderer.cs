using System;
using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;
using UnityEngine;

namespace Diablerie.Engine.World
{
    public class WorldRenderer : MonoBehaviour
    {
        private Dictionary<DT1.Tile, Mesh> meshCache = new Dictionary<DT1.Tile, Mesh>();
        
        private void Start()
        {
            var grid = WorldState.instance.Grid;
            for (int layerIndex = 0; layerIndex < grid.Floors.Length; ++layerIndex)
            {
                var floors = grid.Floors[layerIndex];
                int i = 0;
                for (int y = 0; y < grid.Height - 1; ++y)
                {
                    for (int x = 0; x < grid.Width - 1; ++x)
                    {
                        var tile = floors[i + x];
                        if (tile == null)
                            continue;

                        CreateTileRenderer(tile, x, y);
                    }
                    i += grid.Width;
                }
            }
            for (int layerIndex = 0; layerIndex < grid.Walls.Length; ++layerIndex)
            {
                var walls = grid.Walls[layerIndex];
                int i = 0;
                for (int y = 0; y < grid.Height - 1; ++y)
                {
                    for (int x = 0; x < grid.Width - 1; ++x)
                    {
                        var tile = walls[i + x];
                        if (tile == null)
                            continue;

                        var renderer = CreateTileRenderer(tile, x, y);
                        PutToPopup(tile, renderer, x, y);
                    }
                    i += grid.Width;
                }
            }

            {
                var shadows = grid.Shadows;
                int i = 0;
                for (int y = 0; y < grid.Height - 1; ++y)
                {
                    for (int x = 0; x < grid.Width - 1; ++x)
                    {
                        var tile = shadows[i + x];
                        if (tile == null)
                            continue;

                        CreateTileRenderer(tile, x, y);
                    }

                    i += grid.Width;
                }
            }
            {
                var specialTiles = grid.SpecialTiles;
                int i = 0;
                for (int y = 0; y < grid.Height - 1; ++y)
                {
                    for (int x = 0; x < grid.Width - 1; ++x)
                    {
                        var tile = specialTiles[i + x];
                        if (tile == null)
                            continue;

                        var renderer = CreateTileRenderer(tile, x, y);
                        renderer.gameObject.layer = UnityLayers.SpecialTiles;
                    }

                    i += grid.Width;
                }
            }
        }

        public Renderer CreateTileRenderer(DT1.Tile tile, int x, int y, int orderInLayer = 0,
            Transform parent = null)
        {
            var pos = Iso.MapTileToWorld(x, y);

            GameObject gameObject = new GameObject();
            gameObject.name = tile.mainIndex + "_" + tile.subIndex + "_" + tile.orientation;
            gameObject.transform.position = pos;
            if (parent)
                gameObject.transform.SetParent(parent);
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            
            if (tile.orientation == 0 || tile.orientation == 15)
            {
                meshRenderer.sortingLayerID = tile.orientation == 0 ? SortingLayers.Floor : SortingLayers.Roof;
                meshRenderer.sortingOrder = orderInLayer;

                gameObject.name += tile.orientation == 0 ? " (floor)" : " (roof)";
            }
            else if (tile.orientation > 15)
            {
                meshRenderer.sortingLayerID = SortingLayers.LowerWall;
                meshRenderer.sortingOrder = orderInLayer;
                gameObject.name += " (lower wall)";
            }
            else
            {
                if (tile.orientation == 13) // shadows
                {
                    meshRenderer.sortingLayerID = SortingLayers.Shadow;
                }

                meshRenderer.sortingOrder = Iso.SortingOrder(pos) - 4;
            }

            meshFilter.mesh = CreateTileMesh(tile);

            meshRenderer.material = tile.material;
            return meshRenderer;
        }

        private Mesh CreateTileMesh(DT1.Tile tile)
        {
            if (meshCache.ContainsKey(tile))
            {
                return meshCache[tile];
            }
            
            var texture = tile.texture;
            float x0 = tile.textureX;
            float y0 = tile.textureY;
            float w = tile.width / Iso.pixelsPerUnit;
            float h = -tile.height / Iso.pixelsPerUnit;
            Mesh mesh = new Mesh();
            Vector3 topLeft;
            
            if (tile.orientation == 0 || tile.orientation == 15)
            {
                topLeft = new Vector3(-1f, 0.5f);
                if (tile.orientation == 15)
                    topLeft.y += tile.roofHeight / Iso.pixelsPerUnit;

                gameObject.name += tile.orientation == 0 ? " (floor)" : " (roof)";
            }
            else if (tile.orientation > 15)
            {
                int upperPart = Math.Min(96, -tile.height);
                y0 -= upperPart;
                topLeft = new Vector3(-1f, upperPart / Iso.pixelsPerUnit - 0.5f);
                gameObject.name += " (lower wall)";
            }
            else
            {
                topLeft = new Vector3(-1f, h - 0.5f);
                y0 += tile.height;
            }
            
            mesh.vertices = new[]
            {
                topLeft,
                topLeft + new Vector3(0, -h),
                topLeft + new Vector3(w, -h),
                topLeft + new Vector3(w, 0)
            };
            mesh.uv = new[]
            {
                new Vector2(x0 / texture.width, -y0 / texture.height),
                new Vector2(x0 / texture.width, (-y0 + tile.height) / texture.height),
                new Vector2((x0 + tile.width) / texture.width, (-y0 + tile.height) / texture.height),
                new Vector2((x0 + tile.width) / texture.width, -y0 / texture.height)
            };
            mesh.triangles = new[] {2, 1, 0, 3, 2, 0};
            
            meshCache.Add(tile, mesh);
            return mesh;
        }

        private void PutToPopup(DT1.Tile tile, Renderer renderer, int x, int y)
        {
            foreach (Popup popup in WorldState.instance.Popups)
            {
                if (popup.revealMainIndex == tile.mainIndex && popup.revealArea.Contains(x, y))
                {
                    popup.roofs.Add(renderer);
                }
            }
        }
    }
}