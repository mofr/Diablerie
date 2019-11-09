using System.Collections;
using UnityEngine;

public class PetController : MonoBehaviour
{
    public Character owner;
    public float roamRadius = 25f;
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
            var newPosition = owner.iso.pos + new Vector2(Random.Range(-roamRadius, roamRadius), Random.Range(-roamRadius, roamRadius));
            character.GoTo(newPosition);
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            while (!isActiveAndEnabled) yield return null;
        }
    }
}
