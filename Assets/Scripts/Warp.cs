using UnityEngine;

public class Warp : Entity
{
    LevelInfo targetLevel;
    LevelWarpInfo warpInfo;
    new Transform transform;
    Vector3 selectOffset;
    Vector3 selectSize;

    public static Warp Create(int x, int y, LevelWarpInfo warpInfo, LevelInfo targetLevel, Transform parent)
    {
        var offset = new Vector3(warpInfo.offsetX, warpInfo.offsetY);
        var pos = new Vector3(x, y) * Iso.SubTileCount - new Vector3(2, 2) + offset;
        pos = Iso.MapToWorld(pos);

        var warpObject = new GameObject(targetLevel.levelWarp);
        warpObject.transform.position = pos;
        warpObject.transform.SetParent(parent);
        var warp = warpObject.AddComponent<Warp>();
        warp.targetLevel = targetLevel;
        warp.warpInfo = warpInfo;
        warp.transform = warpObject.transform;
        warp.selectSize = new Vector3(warpInfo.selectDX, warpInfo.selectDY) / Iso.pixelsPerUnit;
        warp.selectOffset = new Vector3(warpInfo.selectX, -warpInfo.selectY) / Iso.pixelsPerUnit;
        warp.selectOffset += new Vector3(warp.selectSize.x, -warp.selectSize.y) / 2;
        return warp;
    }

    public override Bounds bounds
    {
        get
        {
            return new Bounds(transform.position + selectOffset, selectSize);
        }
    }

    public override bool selected
    {
        get { return false; }
        set { }
    }

    public override Vector2 nameOffset
    {
        get { return new Vector2(warpInfo.selectX + warpInfo.selectDX / 2, warpInfo.selectDY / 2); }
    }

    private void OnRenderObject()
    {
        MouseSelection.Submit(this);
    }
}
