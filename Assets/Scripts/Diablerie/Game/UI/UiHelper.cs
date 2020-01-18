using Diablerie.Engine;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using UnityEngine;

namespace Diablerie.Game.UI
{
    public static class UiHelper
    {
        public static GameObject CreateAnimatedObject(string name, string spritePath = "", PaletteType paletteType = PaletteType.Act1, bool loop = true, bool hideOnFinish = false, int sortingOrder = 0)
        {
            var gameObject = new GameObject(name);
            var animator = gameObject.AddComponent<SpriteAnimator>();
            animator.loop = loop;
            animator.hideOnFinish = hideOnFinish;
            animator.Renderer.sortingOrder = sortingOrder;

            if (!string.IsNullOrEmpty(spritePath))
            {
                var sprite = Spritesheet.Load(spritePath, paletteType);
                if (sprite != null)
                {
                    animator.SetSprites(sprite.GetSprites(0));
                }
            }
            
            return gameObject;
        }

        public static Vector3 ScreenToWorldPoint(Vector3 position, int z = 0)
        {
            if (Camera.main == null)
            {
                return new Vector3(0, 0, z);
            }
            
            var result = Camera.main.ScreenToWorldPoint(position);
            result.z = z;

            return result;
        }
    }
}