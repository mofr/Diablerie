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
        private Character character;
        private Character target;
        readonly float viewRadius = 25f;
        readonly float maxAgroDistance = 40f;
        bool tauntSaid = false;

        void Awake()
        {
            character = GetComponent<Character>();
        }

        void OnEnable()
        {
            StartCoroutine(Roam());
        }

        private IEnumerator Roam()
        {
            while (!target)
            {
                Character nearestEnemy = AIUtils.GetNearestEnemy(character, viewRadius);
                if (nearestEnemy != null)
                {
                    Attack(nearestEnemy);
                    yield break;
                }

                var newPosition = character.iso.pos + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
                character.GoTo(newPosition);
                yield return new WaitForSeconds(Random.Range(1f, 2f));
            }
        }

        private void Attack(Character target)
        {
            if (!tauntSaid)
            {
                AudioManager.instance.Play(character.monStat.sound.taunt, character.transform);
                tauntSaid = true;
            }

            this.target = target;
            StartCoroutine(Attack());
        }

        private bool IsAttackable(Character target)
        {
            bool targetTooFar = Vector2.Distance(target.iso.pos, character.iso.pos) > maxAgroDistance;
            return !targetTooFar && AIUtils.IsAttackable(character, target);
        }
    
        private IEnumerator Attack()
        {
            while (true)
            {
                if (IsAttackable(target))
                {
                    character.UseSkill(SkillInfo.Attack, target);
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
