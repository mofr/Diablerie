using System.Collections;
using Diablerie.Engine.AI;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game.AI
{
    public class PetController : MonoBehaviour
    {
        public Unit owner;
        public float roamRadius = 12f;
        public float agroRadius = 15f;
        public float agroOwnerRadius = 25f;
        public float maxOwnerDistance = 32f;
        private Unit _unit;
        private Unit target;
        private Vector2 _maintainOffset;

        void Awake()
        {
            _unit = GetComponent<Unit>();
        }

        void OnEnable()
        {
            StartCoroutine(UpdateOffset());
            StartCoroutine(LookAround());
            StartCoroutine(Roam());
        }

        private IEnumerator UpdateOffset()
        {
            while (!target)
            {
                if (_maintainOffset == Vector2.zero)
                {
                    if (owner == null)
                        yield return null;
                    _maintainOffset = _unit.iso.pos - owner.iso.pos;
                }
                else
                {
                    _maintainOffset = new Vector2(Random.Range(-roamRadius, roamRadius), Random.Range(-roamRadius, roamRadius));
                }
                yield return new WaitForSeconds(Random.Range(10f, 15f));
            }
        }

        private IEnumerator LookAround()
        {
            while (!target)
            {
                _unit.LookAt(_unit.iso.pos + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
                yield return new WaitForSeconds(Random.Range(1f, 2f));
            }
        }

        private IEnumerator Roam()
        {
            while (!target)
            {
                if (owner == null)
                    yield return null;
                if (!IsOwnerTooFar())
                {
                    Unit nearestEnemy = AIUtils.GetNearestEnemy(_unit, agroRadius);
                    if (nearestEnemy == null)
                        nearestEnemy = AIUtils.GetNearestEnemy(owner, agroOwnerRadius);
                    if (nearestEnemy != null)
                    {
                        Attack(nearestEnemy);
                        yield break;
                    }
                }

                var newPosition = owner.iso.pos + _maintainOffset;
                _unit.GoTo(newPosition);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        private bool IsOwnerTooFar()
        {
            return Vector2.Distance(_unit.iso.pos, owner.iso.pos) > maxOwnerDistance;
        }

        private void Attack(Unit target)
        {
            this.target = target;
            StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            while (true)
            {
                if (AIUtils.IsAttackable(_unit, target) && !IsOwnerTooFar())
                {
                    _unit.UseSkill(SkillInfo.Attack, target);
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    target = null;
                    StartCoroutine(UpdateOffset());
                    StartCoroutine(LookAround());
                    StartCoroutine(Roam());
                    yield break;
                }
            }
        }
    }
}
