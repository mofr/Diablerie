using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    Character character;
    Iso iso;
    Character target;
    readonly float viewRadius = 6f;
    readonly float maxAgroDistance = 15f;
    bool tauntSaid = false;

    static Collider2D[] visibleColliders = new Collider2D[100];

	void Awake()
    {
        iso = GetComponent<Iso>();
        character = GetComponent<Character>();
    }

    void OnEnable()
    {
        StartCoroutine(Roam());
    }

    private IEnumerator Roam()
    {
        while (!this.target)
        {
            int visibleCount = Physics2D.OverlapCircleNonAlloc(transform.position, viewRadius, visibleColliders);
            for (int i = 0; i < visibleCount; ++i)
            {
                var collider = visibleColliders[i];
                var visibleCharacter = collider.GetComponent<Character>();
                if (visibleCharacter == null)
                    continue;
                if (IsAttackable(visibleCharacter))
                {
                    if (!tauntSaid)
                    {
                        AudioManager.instance.Play(character.monStat.sound.taunt, character.transform);
                        tauntSaid = true;
                    }
                    Attack(visibleCharacter);
                    yield break;
                }
            }

            var target = iso.pos + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            character.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            while (!isActiveAndEnabled) yield return null;
        }
    }

    private bool IsAttackable(Character target)
    {
        bool targetTooFar = Vector2.Distance(target.iso.pos, iso.pos) > maxAgroDistance;
        bool targetDead = target.Mode == "DT" || target.Mode == "DD";
        bool allied = target.party == character.party;
        return !targetTooFar && !targetDead && !allied;
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
            if (Random.Range(0, 100) > 60)
            {
                character.GoTo(iso.pos);
                yield return new WaitForSeconds(Random.Range(0.5f, 0.7f));
            }

            character.UseSkill(SkillInfo.Attack, target);
            yield return new WaitForSeconds(Random.Range(1.5f, 2f));

            if (!IsAttackable(target))
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                target = null;
                StartCoroutine(Roam());
                yield break;
            }
        }
    }
}
