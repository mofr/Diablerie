using UnityEngine;

class MouseSelection : MonoBehaviour
{
    [HideInInspector]
    static public StaticObject current;
    static StaticObject previous;
    static Vector3 mousePos;

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

    static public void Submit(StaticObject obj, Bounds bounds)
    {
        if (bounds.Contains(mousePos))
        {
            current = obj;
        }
    }
}
