using Diablerie.Engine.Datasheets;

namespace Diablerie.Engine
{
    public static class EngineData
    {
        public static void LoadAll()
        {
            Translation.Load();
            SoundInfo.Load();
            SoundEnvironment.Load();
            ObjectInfo.Load();
            BodyLoc.Load();
            ExpTable.Load();
            LevelType.Load();
            LevelWarpInfo.Load();
            LevelPreset.Load();
            LevelMazeInfo.Load();
            LevelInfo.Load();
            OverlayInfo.Load();
            MissileInfo.Load();
            ItemStat.Load();
            ItemRatio.Load();
            ItemType.Load();
            ItemPropertyInfo.Load();
            ItemSet.Load();
            UniqueItem.Load();
            SetItem.Load();
            TreasureClass.Load();
            MagicAffix.Load();
            CharStatsInfo.Load();
            MonLvl.Load();
            MonPreset.Load();
            MonSound.Load();
            MonStatsExtended.Load();
            MonStat.Load();
            SuperUnique.Load();
            SkillDescription.Load();
            SkillInfo.Load();
            SpawnPreset.Load();
            StateInfo.Load();
        }
    }
}
