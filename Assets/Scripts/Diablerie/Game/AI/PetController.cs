using System.Collections;
using Diablerie.Engine.AI;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Game.AI
{
    public class PetController : MonoBehaviour
    {
        public Character owner;
        public float roamRadius = 12f;
        public float agroRadius = 15f;
        public float agroOwnerRadius = 25f;
        public float maxOwnerDistance = 32f;
        private Character character;
        private Character target;
        private Vector2 _maintainOffset;

        void Awake()
        {
            character = GetComponent<Character>();
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
                    _maintainOffset = character.iso.pos - owner.iso.pos;
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
                character.LookAt(character.iso.pos + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));
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
                    Character nearestEnemy = AIUtils.GetNearestEnemy(character, agroRadius);
                    if (nearestEnemy == null)
                        nearestEnemy = AIUtils.GetNearestEnemy(owner, agroOwnerRadius);
                    if (nearestEnemy != null)
                    {
                        Attack(nearestEnemy);
                        yield break;
                    }
                }

                var newPosition = owner.iso.pos + _maintainOffset;
                character.GoTo(newPosition);
                yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
            }
        }

        private bool IsOwnerTooFar()
        {
            return Vector2.Distance(character.iso.pos, owner.iso.pos) > maxOwnerDistance;
        }

        private void Attack(Character target)
        {
            this.target = target;
            StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            while (true)
            {
                if (AIUtils.IsAttackable(character, target) && !IsOwnerTooFar())
                {
                    character.UseSkill(SkillInfo.Attack, target);
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
