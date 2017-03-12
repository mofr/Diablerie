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

    void OnGUI()
    {
        if (current != null)
        {
            GUI.skin.label.alignment = TextAnchor.LowerCenter;
            GUI.skin.font = selectionFont;
            var pos = Camera.main.WorldToScreenPoint(currentPosition);
            pos.y = Camera.main.pixelHeight - pos.y + current.nameOffset;
            const int width = 500;
            const int height = 100;
            GUI.Label(new Rect(pos - new Vector3(width / 2, height), new Vector2(width, height)), current.name);
            EnemyBar.instance.character = current.GetComponent<Character>();
        }
        else
        {
            EnemyBar.instance.character = null;
        }
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
