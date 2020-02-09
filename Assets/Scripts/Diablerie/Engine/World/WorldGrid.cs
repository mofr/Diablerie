using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.Utility;

namespace Diablerie.Engine.World
{
    public class WorldGrid
    {
        private DT1.Tile[][] floors = new DT1.Tile[4][];
        private DT1.Tile[][] walls = new DT1.Tile[4][];
        private DT1.Tile[] shadows;
        private DT1.Tile[] specialTiles;
        private int width;
        private int height;
        
        public delegate void OnResetHandler();
        public event OnResetHandler OnReset;

        public WorldGrid(int width, int height)
        {
            this.width = width;
            this.height = height;
            Reset();
        }

        public void Reset()
        {
            int cellCount = width * height;
            
            for (int i = 0; i < floors.Length; ++i)
            {
                floors[i] = new DT1.Tile[cellCount];
            }
            for (int i = 0; i < walls.Length; ++i)
            {
                walls[i] = new DT1.Tile[cellCount];
            }
            shadows = new DT1.Tile[cellCount];
            specialTiles = new DT1.Tile[cellCount];
            
            OnReset?.Invoke();
        }

        public void PutFloor(DT1.Tile tile, int x, int y, int layerIndex)
        {
            floors[layerIndex][y * width + x] = tile;
            ApplyTileCollisions(tile, x, y);
        }

        public void PutWall(DT1.Tile tile, int x, int y, int layerIndex)
        {
            walls[layerIndex][y * width + x] = tile;
            ApplyTileCollisions(tile, x, y);
        }

        public void PutShadow(DT1.Tile tile, int x, int y)
        {
            shadows[y * width + x] = tile;
            ApplyTileCollisions(tile, x, y);
        }

        public void PutSpecialTile(DT1.Tile tile, int x, int y)
        {
            specialTiles[y * width + x] = tile;
        }

        public int Width => width;
        public int Height => height;
        public DT1.Tile[][] Floors => floors;
        public DT1.Tile[][] Walls => walls;
        public DT1.Tile[] Shadows => shadows;
        public DT1.Tile[] SpecialTiles => specialTiles;
        
        private static void ApplyTileCollisions(DT1.Tile tile, int x, int y)
        {
            var pos = Iso.MapTileToWorld(x, y);
            var collisionMapOffset = Iso.Snap(Iso.MapToIso(pos));
            int flagIndex = 0;
            DT1.BlockFlags mask = DT1.BlockFlags.Walk | DT1.BlockFlags.PlayerWalk;
            for (int dy = 2; dy > -3; --dy)
            {
                for (int dx = -2; dx < 3; ++dx, ++flagIndex)
                {
                    Vector2i subCellPos = collisionMapOffset + new Vector2i(dx, dy);
                    bool passable = (tile.flags[flagIndex] & mask) == 0;
                    CollisionLayers blockedLayers = passable ? CollisionLayers.None : CollisionLayers.Walk;
                    if (tile.orientation == 0)
                    {
                        CollisionMap.SetBlocked(subCellPos, blockedLayers);
                    }
                    else if (CollisionMap.Passable(subCellPos, CollisionLayers.Walk) && !passable)
                    {
                        CollisionMap.SetBlocked(subCellPos, blockedLayers);
                    }
                }
            }
        }
    }
}
