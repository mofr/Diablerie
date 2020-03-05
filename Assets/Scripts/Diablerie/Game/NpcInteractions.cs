using Diablerie.Engine;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game
{
    public class NpcInteractions : MonoBehaviour
    {
        public void Awake()
        {
            Events.UnitInteractionStarted += OnInteract;
        }
        
        public void OnInteract(Unit target, Unit initiator)
        {
            if (target.monStat == null || !target.monStat.npc)
                return;
            Debug.Log("Interact with " + target);
            initiator.LookAt(target.iso.pos);
            var greetingSound = SoundInfo.Find(target.monStat.id + "_greeting_1");
            AudioManager.instance.Play(greetingSound, target.transform);
        }
    }
}
