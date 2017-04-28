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
                UI.HideLabel();
            }
            else
            {
                EnemyBar.instance.character = null;
                var labelPosition = current.transform.position + (Vector3)current.titleOffset / Iso.pixelsPerUnit;
                UI.ShowLabel(labelPosition, current.title);
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

        Bounds bounds = entity.bounds;

        if (Input.GetMouseButton(0))
        {
            if (entity == current)
            {
                currentPosition = bounds.center;
            }
            return;
        }

        bounds.Expand(Expand);
        if (!bounds.Contains(mousePos))
            return;

        bool betterMatch = current == null || Tools.manhattanDistance(mousePos, bounds.center) < Tools.manhattanDistance(mousePos, currentPosition);
        if (betterMatch)
        {
            current = entity;
            currentPosition = bounds.center;
        }
    }
}
