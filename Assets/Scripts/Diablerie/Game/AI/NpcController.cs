using System.Collections;
using Diablerie.Engine.Entities;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    private Character _character;
    private Vector2 _initialPosition;
    private Coroutine _currentAction;

    void Awake()
    {
        _character = GetComponent<Character>();
    }

    void OnEnable()
    {
        _initialPosition = _character.iso.pos;
        _currentAction = StartCoroutine(WalkAround());
    }

    void OnDisable()
    {
        StopCoroutine(_currentAction);
    }

    IEnumerator WalkAround()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            var target = _initialPosition + new Vector2(Random.Range(-8f, 8f), Random.Range(-8f, 8f));
            _character.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
}
