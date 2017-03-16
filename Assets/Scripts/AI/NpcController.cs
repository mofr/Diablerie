using System.Collections;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    Character character;
    Iso iso;
    Vector2 initialPosition;

    void Awake()
    {
        iso = GetComponent<Iso>();
    }

    void Start()
    {
        initialPosition = iso.pos;
        character = GetComponent<Character>();
        StartCoroutine(WalkAround());
    }

    IEnumerator WalkAround()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            var target = initialPosition + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            character.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            while (!isActiveAndEnabled) yield return null;
        }
    }
}
