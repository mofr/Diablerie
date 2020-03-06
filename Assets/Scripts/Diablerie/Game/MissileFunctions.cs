using Diablerie.Engine;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game
{
    public class MissileFunctions : MonoBehaviour
    {
        public void Awake()
        {
            Events.MissileMoved += OnMove;
        }
        
        private static void OnMove(Missile missile)
        {
            var info = missile.Info;
            var lifeTime = missile.LifeTime;
            var originator = missile.Originator;
            var pos = missile.Iso.pos;
            
            if (missile.Info.serverDoFunc == 15)
            {
                // Frozen orb
                int frequency = info.parameters[0].value * 25;
                float spawnPeriod = 1.0f / frequency;
                float directionIncrement = info.parameters[1].value * 25 * Mathf.PI;
                int missilesToSpawn = (int)((lifeTime + Time.deltaTime) / spawnPeriod) - (int)(lifeTime / spawnPeriod);
                for (int i = 0; i < missilesToSpawn; ++i)
                {
                    var dir = new Vector2(1, 0);
                    var rot = Quaternion.AngleAxis(lifeTime * directionIncrement, new Vector3(0, 0, 1));
                    var offset = (Vector2) (rot * dir);
                    Missile.Create(info.clientSubMissileId[0], pos, pos + offset, originator);
                }
            }
        }
    }
}
