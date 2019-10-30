using UnityEngine;

public class DebugCollisionMap : MonoBehaviour
{
    public void Update()
    {
        DrawDebugCellGrid();
    }
    
    private void DrawDebugCellGrid()
    {
        Color occupiedColor = new Color(1, 0, 0, 0.3f);
        Color passableColor = new Color(1, 1, 1, 0.03f);
        Vector2i origin = Iso.Snap(Iso.MapToIso(Camera.main.transform.position));
        int debugWidth = 100;
        int debugHeight = 100;
        origin.x -= debugWidth / 2;
        origin.y -= debugHeight / 2;
        for (int y = 0; y < debugHeight; ++y)
        {
            for (int x = 0; x < debugWidth; ++x)
            {
                var pos = origin + new Vector2i(x, y);
                bool passable = CollisionMap.Passable(pos);
                Color color = passable ? passableColor : occupiedColor;
                Iso.DebugDrawTile(pos, color, 0.9f);
            }
        }
    }
}
