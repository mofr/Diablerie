using System.Collections;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public Character owner;
    public float roamRadius = 15f;
    public float agroRadius = 3f;
    public float agroOwnerRadius = 5f;
    private Character character;
    private Character target;

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
            if (owner == null)
                yield return null;
            Character nearestEnemy = AIUtils.GetNearestEnemy(character, agroRadius);
            if (nearestEnemy == null)
                nearestEnemy = AIUtils.GetNearestEnemy(owner, agroOwnerRadius);
            if (nearestEnemy != null)
            {
                Attack(nearestEnemy);
                yield break;
            }
            var newPosition = owner.iso.pos + new Vector2(Random.Range(-roamRadius, roamRadius), Random.Range(-roamRadius, roamRadius));
            character.GoTo(newPosition);
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            while (!isActiveAndEnabled) yield return null;
        }
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
            if (AIUtils.IsAttackable(character, target))
            {
                character.UseSkill(SkillInfo.Attack, target);
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                target = null;
                StartCoroutine(Roam());
                yield break;
            }
        }
    }
}
