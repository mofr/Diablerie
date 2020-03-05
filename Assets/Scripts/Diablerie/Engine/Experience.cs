using Diablerie.Engine.Entities;
using Diablerie.Engine.World;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Experience : MonoBehaviour
    {
        void Start()
        {
            Events.UnitDied += UnitDeath;
        }

        void UnitDeath(Unit target, Unit killer)
        {
            var player = WorldState.instance.Player;
            
            if (target.monStat != null && killer == player.unit)
            {
                player.charStat.AddExp(target.monStat.stats[0].exp);
            }
        }
    }
}
