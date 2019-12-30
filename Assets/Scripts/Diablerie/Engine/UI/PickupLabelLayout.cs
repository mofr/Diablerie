using System.Collections.Generic;
using System.Linq;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class PickupLabelLayout
    {
        private readonly List<Rect> placedRects = new List<Rect>();
        private float verticalPadding;

        public PickupLabelLayout(float verticalPaddingInPixels)
        {
            verticalPadding = verticalPaddingInPixels / Iso.pixelsPerUnit;
        }

        public void Arrange(IReadOnlyCollection<KeyValuePair<Pickup, Label>> labels)
        {
            var sortedLabels = labels.OrderBy(x => x.Key.transform.position.y);
            placedRects.Clear();
            foreach (KeyValuePair<Pickup, Label> entry in sortedLabels)
            {
                Pickup pickup = entry.Key;
                Label label = entry.Value;
                var position = pickup.transform.position + (Vector3) pickup.titleOffset / Iso.pixelsPerUnit;
                label.Show(position, pickup.title);
                var rect = new Rect(position, label.RectTransform.rect.size / Iso.pixelsPerUnit);
                Vector3 offset = PutRect(rect);
                label.Show(position + offset, pickup.title);
            }
        }

        private Vector2 PutRect(Rect rect)
        {
            var initialRect = rect;
            for (int i = 0; i < placedRects.Count; i++)
            {
                Rect placedRect = placedRects[i];
                if (rect.Overlaps(placedRect))
                {
                    rect.y = placedRect.yMax + verticalPadding;
                }
            }
            
            placedRects.Add(rect);
            return rect.position - initialRect.position;
        }
    }
}