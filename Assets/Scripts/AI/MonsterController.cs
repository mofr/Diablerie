using System.Collections;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    Character character;
    Iso iso;
    Character target;
    readonly float viewRadius = 6f;
    readonly float maxAgroDistance = 15f;

    static Collider2D[] visibleColliders = new Collider2D[100];

	void Awake() {
        iso = GetComponent<Iso>();
    }

    void Start()
    {
        character = GetComponent<Character>();
        character.OnTakeDamage += OnTakeDamage;
        StartCoroutine(Roam());
    }

    void OnTakeDamage(Character originator, int damage)
    {
        Attack(originator);
    }

    IEnumerator Roam()
    {
        yield return new WaitForEndOfFrame();
        while (!this.target)
        {
            var target = iso.pos + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            character.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            while (!isActiveAndEnabled) yield return null;

            int visibleCount = Physics2D.OverlapCircleNonAlloc(transform.position, viewRadius, visibleColliders);
            for (int i = 0; i < visibleCount; ++i)
            {
                var collider = visibleColliders[i];
                var visibleCharacter = collider.GetComponent<Character>();
                if (visibleCharacter == null)
                    continue;
                if (visibleCharacter.gameObject.name == "Player")
                {
                    Attack(visibleCharacter);
                }
            }
        }
    }

    void Attack(Character target)
    {
        this.target = target;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            if (Random.Range(0, 100) > 60)
            {
                character.GoTo(iso.pos);
                yield return new WaitForSeconds(Random.Range(0.5f, 0.7f));
            }

            character.Attack(target);
            yield return new WaitForSeconds(Random.Range(1.5f, 2f));

            Iso targetIso = target.GetComponent<Iso>();
            if (Vector2.Distance(targetIso.pos, iso.pos) > maxAgroDistance)
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
                target = null;
                StartCoroutine(Roam());
                break;
            }
        }
    }
}
