using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Diablerie.Engine.Datasheets;
using Debug = UnityEngine.Debug;

namespace Diablerie.Engine
{
    public static class EngineData
    {
        public class LoadProgress
        {
            public int totalCount;
            public int doneCount;
            public bool finished;
        }

        public static LoadProgress LoadAll()
        {
            var progress = new LoadProgress();
            Task.Run(() => LoadDatasheets(progress));
            return progress;
        }

        private static void LoadDatasheets(LoadProgress progress)
        {
            var sw = Stopwatch.StartNew();
            List<Action> actions = new List<Action>();
            actions.Add(Translation.Load);
            actions.Add(SoundInfo.Load);
            actions.Add(SoundEnvironment.Load);
            actions.Add(ObjectInfo.Load);
            actions.Add(BodyLoc.Load);
            actions.Add(ExpTable.Load);
            actions.Add(LevelType.Load);
            actions.Add(LevelWarpInfo.Load);
            actions.Add(LevelPreset.Load);
            actions.Add(LevelMazeInfo.Load);
            actions.Add(LevelInfo.Load);
            actions.Add(OverlayInfo.Load);
            actions.Add(MissileInfo.Load);
            actions.Add(ItemStat.Load);
            actions.Add(ItemRatio.Load);
            actions.Add(ItemType.Load);
            actions.Add(ItemPropertyInfo.Load);
            actions.Add(ItemSet.Load);
            actions.Add(UniqueItem.Load);
            actions.Add(SetItem.Load);
            actions.Add(TreasureClass.Load);
            actions.Add(MagicAffix.Load);
            actions.Add(CharStatsInfo.Load);
            actions.Add(MonLvl.Load);
            actions.Add(MonPreset.Load);
            actions.Add(MonSound.Load);
            actions.Add(MonStatsExtended.Load);
            actions.Add(MonStat.Load);
            actions.Add(SuperUnique.Load);
            actions.Add(SkillDescription.Load);
            actions.Add(SkillInfo.Load);
            actions.Add(SpawnPreset.Load);
            actions.Add(StateInfo.Load);
            progress.totalCount = actions.Count;
            foreach (Action action in actions)
            {
                UnityEngine.Profiling.Profiler.BeginSample("LoadDatasheet " + progress.doneCount);
                action();
                progress.doneCount++;
                UnityEngine.Profiling.Profiler.EndSample();
            }
            progress.finished = true;
            Debug.Log("All txt files loaded in " + sw.ElapsedMilliseconds + " ms");
        }
    }
}