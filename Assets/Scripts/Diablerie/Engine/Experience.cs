using Diablerie.Engine.Entities;
using Diablerie.Engine.World;
using UnityEngine;

namespace Diablerie.Engine
{
    public class Experience : MonoBehaviour
    {
        void Start()
        {
            Character.OnDeath += OnCharacterDeath;
        }

        void OnCharacterDeath(Character target, Character killer)
        {
            var player = WorldState.instance.Player;
            
            if (target.monStat != null && killer == player.character)
            {
                player.charStat.AddExp(target.monStat.stats[0].exp);
            }
        }
    }
}
