using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour {

    Character character;
    Iso iso;
    Character target;

	void Awake() {
        character = GetComponent<Character>();
        iso = GetComponent<Iso>();
    }

    void Start()
    {
        character.OnTakeDamage += OnTakeDamage;
        StartCoroutine(Roam());
    }

    void OnTakeDamage(Character originator, int damage)
    {
        StopCoroutine("Roam");
        target = originator;
        StartCoroutine(Attack());
    }

    IEnumerator Roam()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            var target = iso.tilePos + new Vector2(Random.Range(-8, 8), Random.Range(-8, 8));
            character.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1, 5));
        }
    }

    IEnumerator Attack()
    {
        while (true)
        {
            character.Attack(target);
            yield return new WaitForSeconds(Random.Range(0.5f, 3));
        }
    }
}
