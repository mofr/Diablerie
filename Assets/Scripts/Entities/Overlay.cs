using UnityEngine;

public class Overlay : MonoBehaviour
{
    new SpriteRenderer renderer;
    SpriteAnimator animator;

    public static Overlay Create(GameObject gameObject, string overlayId)
    {
        var overlayInfo = OverlayInfo.Find(overlayId);
        if (overlayInfo == null)
        {
            Debug.LogWarning("overlay not found: " + overlayId);
            return null;
        }

        return Create(gameObject, overlayInfo);
    }

    public static Overlay Create(GameObject gameObject, OverlayInfo overlayInfo)
    {
        // todo overlay objects recycler
        var overlayObject = new GameObject(overlayInfo.id + " (overlay)");
        overlayObject.transform.SetParent(gameObject.transform, false);
        overlayObject.transform.localPosition = new Vector3(0, 0, -2);
        var spritesheet = Spritesheet.Load(overlayInfo.spritesheetFilename);
        var overlay = overlayObject.AddComponent<Overlay>();
        overlay.animator = overlayObject.AddComponent<SpriteAnimator>();
        overlay.animator.loop = false;
        overlay.animator.sprites = spritesheet.GetSprites(0);
        overlay.animator.fps = overlayInfo.fps * 1.5f; // todo connect to spell cast rate
        overlay.renderer = overlayObject.GetComponent<SpriteRenderer>();
        overlay.renderer.material = Materials.softAdditive;
        return overlay;
    }
    
    void LateUpdate()
    {
        renderer.sortingOrder = Iso.SortingOrder(transform.position);
        if (animator.finished)
            Destroy(gameObject);
    }
}
