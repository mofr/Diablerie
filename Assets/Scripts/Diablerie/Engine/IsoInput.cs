using UnityEngine;

namespace Diablerie.Engine
{
    public class IsoInput : MonoBehaviour
    {
        public static Vector2 mousePosition;
        public static Vector3 mouseTile;

        void Update()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = Iso.MapToIso(mousePos);
            mouseTile = Iso.Snap(mousePosition);
        }
    }
}
