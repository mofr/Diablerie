using System.Collections;
using Diablerie.Engine;
using Diablerie.Engine.AI;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game.AI
{
    public class MonsterController : MonoBehaviour
    {
        private Unit _unit;
        private Unit target;
        readonly float viewRadius = 25f;
        readonly float maxAgroDistance = 40f;
        bool tauntSaid = false;

        void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        void OnEnable()
        {
            StartCoroutine(Roam());
        }

        private IEnumerator Roam()
        {
            while (!target)
            {
                Unit nearestEnemy = AIUtils.GetNearestEnemy(_unit, viewRadius);
                if (nearestEnemy != null)
                {
                    Attack(nearestEnemy);
                    yield break;
                }

                var newPosition = _unit.iso.pos + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
                _unit.GoTo(newPosition);
                yield return new WaitForSeconds(Random.Range(1f, 2f));
            }
        }

        private void Attack(Unit target)
        {
            if (!tauntSaid)
            {
                AudioManager.instance.Play(_unit.monStat.sound.taunt, _unit.transform);
                tauntSaid = true;
            }

            this.target = target;
            StartCoroutine(Attack());
        }

        private bool IsAttackable(Unit target)
        {
            bool targetTooFar = Vector2.Distance(target.iso.pos, _unit.iso.pos) > maxAgroDistance;
            return !targetTooFar && AIUtils.IsAttackable(_unit, target);
        }
    
        private IEnumerator Attack()
        {
            while (true)
            {
                if (IsAttackable(target))
                {
                    _unit.UseSkill(SkillInfo.Attack, target);
                    yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                }
                else
                {
                    yield return null;
                    target = null;
                    StartCoroutine(Roam());
                    yield break;
                }
            }
        }
    }
}
