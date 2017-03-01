using UnityEngine;

class MouseSelection : MonoBehaviour
{
    public Font selectionFont;

    [HideInInspector]
    static StaticObject current;
    static StaticObject previous;
    static Vector3 mousePos;
    static Bounds bounds;

    void Update()
    {
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
            var pos = Camera.main.WorldToScreenPoint(bounds.center);
            pos.y = Camera.main.pixelHeight - pos.y + current.info.nameOffset;
            const int width = 500;
            const int height = 100;
            GUI.Label(new Rect(pos - new Vector3(width / 2, height), new Vector2(width, height)), current.info.name);
        }
    }

    static public void Submit(StaticObject obj, Bounds bounds)
    {
        bool betterMatch = current == null || bounds.center.y < MouseSelection.bounds.center.y;
        if (betterMatch && bounds.Contains(mousePos))
        {
            current = obj;
            MouseSelection.bounds = bounds;
        }
    }
}
