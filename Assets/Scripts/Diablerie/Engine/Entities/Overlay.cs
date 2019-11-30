using Diablerie.Engine.Datasheets;
using UnityEngine;

namespace Diablerie.Engine.Entities
{
    public class Overlay : MonoBehaviour
    {
        new SpriteRenderer renderer;
        SpriteAnimator animator;

        public static Overlay Create(GameObject gameObject, string overlayId, bool loop, float speed = 1.0f)
        {
            var overlayInfo = OverlayInfo.Find(overlayId);
            if (overlayInfo == null)
            {
                Debug.LogWarning("overlay not found: " + overlayId);
                return null;
            }

            return Create(gameObject, overlayInfo, loop, speed);
        }

        public static Overlay Create(GameObject gameObject, OverlayInfo overlayInfo, bool loop, float speed = 1.0f)
        {
            // todo overlay objects recycler
            var overlayObject = new GameObject(overlayInfo.id + " (overlay)");
            overlayObject.transform.SetParent(gameObject.transform, false);
            float zOffset = overlayInfo.preDraw ? 0.5f : -0.5f;
            overlayObject.transform.localPosition = new Vector3(0, 0, zOffset);
            var spritesheet = Spritesheet.Load(overlayInfo.spritesheetFilename);
            var overlay = overlayObject.AddComponent<Overlay>();
            overlay.animator = overlayObject.AddComponent<SpriteAnimator>();
            overlay.animator.loop = loop;
            overlay.animator.sprites = spritesheet.GetSprites(0);
            overlay.animator.fps = overlayInfo.fps * speed * 1.5f; // 1.5 multiplier is set to match original animation speed
            if (!loop)
                overlay.animator.OnFinish += overlay.OnAnimationFinish;
            overlay.renderer = overlayObject.GetComponent<SpriteRenderer>();
            overlay.renderer.material = Materials.softAdditive;
            return overlay;
        }

        void OnAnimationFinish()
        {
            Destroy(gameObject);
        }
    
        void LateUpdate()
        {
            renderer.sortingOrder = Iso.SortingOrder(transform.position);
        }
    }
}
