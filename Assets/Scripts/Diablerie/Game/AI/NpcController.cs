using System.Collections;
using Diablerie.Engine;
using Diablerie.Engine.Entities;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    private Unit _unit;
    private Vector2 _initialPosition;
    private Coroutine _currentAction;

    void Awake()
    {
        _unit = GetComponent<Unit>();
        Events.UnitInteractionStarted += OnInteractionStarted;
    }

    private void OnInteractionStarted(Unit target, Unit initiator)
    {
        if (target != _unit)
            return;
        
        StopCoroutine(_currentAction);
        _currentAction = StartCoroutine(Interact(initiator));
    }

    void OnEnable()
    {
        _initialPosition = _unit.iso.pos;
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
            _unit.GoTo(target);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }
    
    IEnumerator Interact(Unit initiator)
    {
        _unit.StopMoving();
        _unit.LookAt(initiator.iso.pos);
        yield return new WaitForSeconds(3f); // TODO wait until interaction is finished
        _currentAction = StartCoroutine(WalkAround());
    }
}
