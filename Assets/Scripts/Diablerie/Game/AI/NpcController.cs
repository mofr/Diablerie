using System.Collections;
using Diablerie.Engine;
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
        Events.CharacterInteractionStarted += OnInteractionStarted;
    }

    private void OnInteractionStarted(Character target, Character initiator)
    {
        if (target != _character)
            return;
        
        StopCoroutine(_currentAction);
        _currentAction = StartCoroutine(Interact(initiator));
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
    
    IEnumerator Interact(Character initiator)
    {
        _character.StopMoving();
        _character.LookAt(initiator.iso.pos);
        yield return new WaitForSeconds(3f); // TODO wait until interaction is finished
        _currentAction = StartCoroutine(WalkAround());
    }
}
