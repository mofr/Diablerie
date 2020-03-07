using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.World
{
    public class Warp : Entity
    {
        LevelWarpInfo info;
        LevelInfo sourceLevel;
        LevelInfo targetLevel;
        new Transform transform;
        Vector3 selectOffset;
        Vector3 selectSize;

        public static Warp Create(int x, int y, LevelWarpInfo warpInfo, LevelInfo sourceLevel, LevelInfo targetLevel, Transform parent)
        {
            var offset = new Vector3(warpInfo.offsetX, warpInfo.offsetY);
            var pos = new Vector3(x, y) * Iso.SubTileCount - new Vector3(2, 2) + offset;
            pos = Iso.MapToWorld(pos);

            var warpObject = new GameObject(targetLevel.levelWarp);
            warpObject.transform.position = pos;
            warpObject.transform.SetParent(parent);
            var warp = warpObject.AddComponent<Warp>();
            warp.sourceLevel = sourceLevel;
            warp.targetLevel = targetLevel;
            warp.info = warpInfo;
            warp.transform = warpObject.transform;
            warp.selectSize = new Vector3(warpInfo.selectDX, warpInfo.selectDY) / Iso.pixelsPerUnit;
            warp.selectOffset = new Vector3(warpInfo.selectX, -warpInfo.selectY) / Iso.pixelsPerUnit;
            warp.selectOffset += new Vector3(warp.selectSize.x, -warp.selectSize.y) / 2;
            warpInfo.instance = warp;
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

        public override Vector2 titleOffset
        {
            get { return new Vector2(info.selectX + info.selectDX / 2, info.selectDY / 2); }
        }

        private Warp FindTargetWarp()
        {
            for (int i = 0; i < targetLevel.vis.Length; ++i)
            {
                if (targetLevel.vis[i] == sourceLevel.id)
                {
                    int warpId = targetLevel.warp[i];
                    Warp warp = LevelWarpInfo.Find(warpId).instance;
                    if (warp != null)
                        return warp;
                }
            }

            return null;
        }

        public override void Operate(Unit unit)
        {
            var targetWarp = FindTargetWarp();
            if (targetWarp == null)
            {
                Debug.LogError("Target warp wasn't found");
                return;
            }

            ScreenFader.SetToBlack();
            ScreenFader.FadeToClear();
            var target = Iso.MapToIso(targetWarp.transform.position);
            unit.InstantMove(target);
            unit.GoTo(target + new Vector3(targetWarp.info.exitWalkX, targetWarp.info.exitWalkY));
        }
    }
}
