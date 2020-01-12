using System;
using System.Collections.Generic;
using System.Linq;
using Diablerie.Engine.Entities;
using Diablerie.Engine.Utility;
using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class PickupLabelLayout
    {
        private readonly SortedList<float, Rect> placedRects = new SortedList<float, Rect>(new DuplicateKeyComparer<float>());
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
                rect.x -= rect.width / 2;
                Rect placedRect = PutRect(rect);
                Vector3 offset = placedRect.position - rect.position;
                label.Show(position + offset, pickup.title);
            }
        }

        private Rect PutRect(Rect rect)
        {
            FitIntoTheScreen(ref rect);
            foreach (Rect placedRect in placedRects.Values)
            {
                if (rect.Overlaps(placedRect))
                {
                    rect.y = placedRect.yMax + verticalPadding;
                }
            }

            placedRects.Add(rect.y, rect);
            return rect;
        }

        private void FitIntoTheScreen(ref Rect rect)
        {
            Vector2 screenSize = Ui.instance.RectTransform.sizeDelta;
            Vector2 minPosition = Camera.main.WorldToViewportPoint(rect.min);
            Vector2 maxPosition = Camera.main.WorldToViewportPoint(rect.max);
            float leftOverflow = Math.Min(minPosition.x, 0);
            float rightOverflow = Math.Max(maxPosition.x - 1, 0);
            float topOverflow = Math.Max(maxPosition.y - 1, 0);
            float horizontalOffset = -(leftOverflow + rightOverflow) * screenSize.x / Iso.pixelsPerUnit;
            float verticalOffset = -topOverflow * screenSize.y / Iso.pixelsPerUnit;
            rect.position += new Vector2(horizontalOffset, verticalOffset);
        }
    }
}