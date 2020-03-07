using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Game
{
    public class StaticObjectFunctions : MonoBehaviour
    {
        private static readonly string[] TreasureClassLetters = new string[] { "A", "B", "C" };
        
        public void Awake()
        {
            Events.StaticObjectOperate += Operate;
        }
        
        private void Operate(StaticObject staticObject, Unit unit)
        {
            Debug.Log(unit.name + " operate " + staticObject.name + " (operateFn " + staticObject.info.operateFn + ")");

            if (staticObject.info.operateFn == 1 // bed, caskets
                || staticObject.info.operateFn == 3 // urns
                || staticObject.info.operateFn == 4 // chests
                || staticObject.info.operateFn == 5 // barrels
                || staticObject.info.operateFn == 14 // crates
                || staticObject.info.operateFn == 51 // jungle objects
            )
            {
                AudioManager.instance.Play("object_chest_large");

                var levelInfo = LevelInfo.sheet[85]; // todo determine current level
                string tc = "Act " + (levelInfo.act + 1);
                var actLevels = LevelInfo.byAct[levelInfo.act];
                int lowest = actLevels[0].id;
                int highest = actLevels[actLevels.Count - 1].id;
                int letterIndex = (levelInfo.id - lowest) / ((highest - lowest + 1) / 3);
                string letter = TreasureClassLetters[letterIndex];
                tc += " Chest " + letter;
                Debug.Log(tc);
                ItemDrop.Drop(tc, staticObject.transform.position, levelInfo.id);
                staticObject.SetMode(StaticObjectMode.Operating);
            }
            else if (staticObject.info.operateFn == 23)
            {
                // waypoint
                if (staticObject.Mode != StaticObjectMode.Operating)
                {
                    AudioManager.instance.Play("object_waypoint_open");
                    staticObject.SetMode(StaticObjectMode.Operating);
                }
            }
            else
            {
                staticObject.SetMode(StaticObjectMode.Operating);
            }
        }
    }
}
