using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.AI
{
    public class AIUtils
    {
        private static Collider2D[] visibleColliders = new Collider2D[300];
    
        public static Character GetNearestEnemy(Character requester, float radius)
        {
            float unityRadius = radius * Iso.tileSize;
            int visibleCount = Physics2D.OverlapCircleNonAlloc(requester.transform.position, unityRadius, visibleColliders);
            for (int i = 0; i < visibleCount; ++i)
            {
                var collider = visibleColliders[i];
                var visibleCharacter = collider.GetComponent<Character>();
                if (visibleCharacter == null)
                    continue;
                if (IsAttackable(requester, visibleCharacter))
                {
                    return visibleCharacter;
                }
            }

            return null;
        }
    
        public static bool IsAttackable(Character attacker, Character target)
        {
            bool targetDead = target.Mode == "DT" || target.Mode == "DD";
            bool allied = target.party == attacker.party;
            return !targetDead && !allied && target.killable;
        }
    }
}
