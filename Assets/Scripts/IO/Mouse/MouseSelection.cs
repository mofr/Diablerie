using UnityEngine;

class MouseSelection : MonoBehaviour
{
    static readonly Vector3 Expand = new Vector3(25, 20) / Iso.pixelsPerUnit;
    public Font selectionFont;

    [HideInInspector]
    static public Entity current;
    static Entity previous;
    static Vector3 mousePos;
    static Vector3 currentPosition;

    void Update()
    {
        if (current != null)
        {
            var character = current.GetComponent<Character>();
            if (character && character.monStat != null && character.monStat.ai != "Npc")
            {
                EnemyBar.instance.character = character;
            }
            else
            {
                var labelPosition = current.transform.position + (Vector3)current.nameOffset / Iso.pixelsPerUnit;
                UI.ShowLabel(labelPosition, current.name);
            }
        }
        else
        {
            EnemyBar.instance.character = null;
            UI.HideLabel();
        }

        if (Input.GetMouseButton(0))
        {
            return;
        }

        if (previous != null)
        {
            previous.selected = false;
        }
        if (current != null)
        {
            current.selected = true;
        }
        previous = current;
        current = null;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    }

    static public void Submit(Entity entity)
    {
        if (entity == PlayerController.instance.character)
            return;

        var position = entity.transform.position;

        if (Input.GetMouseButton(0))
        {
            if(entity == current)
            {
                currentPosition = position;
            }
            return;
        }

        bool betterMatch = current == null || position.y < currentPosition.y;
        if (betterMatch)
        {
            Bounds bounds = entity.bounds;
            bounds.Expand(Expand);
            if (bounds.Contains(mousePos))
            {
                current = entity;
                currentPosition = position;
            }
        }
    }
}
