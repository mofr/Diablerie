using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.AI
{
    public class AIUtils
    {
        private static Collider2D[] visibleColliders = new Collider2D[300];
    
        public static Unit GetNearestEnemy(Unit requester, float radius)
        {
            float unityRadius = radius * Iso.tileSize;
            int visibleCount = Physics2D.OverlapCircleNonAlloc(requester.transform.position, unityRadius, visibleColliders);
            Unit nearest = null;
            float minDistance = radius;
            for (int i = 0; i < visibleCount; ++i)
            {
                var collider = visibleColliders[i];
                var visibleUnit = collider.GetComponent<Unit>();
                if (visibleUnit == null)
                    continue;
                if (IsAttackable(requester, visibleUnit))
                {
                    float distance = Vector2.Distance(visibleUnit.iso.pos, requester.iso.pos);
                    if (nearest == null || distance < minDistance)
                        nearest = visibleUnit;
                }
            }

            return nearest;
        }
    
        public static bool IsAttackable(Unit attacker, Unit target)
        {
            bool targetDead = target.Mode == "DT" || target.Mode == "DD";
            bool allied = target.party == attacker.party;
            return !targetDead && !allied && target.killable;
        }
    }
}
