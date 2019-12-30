using System.Collections.Generic;
using Diablerie.Engine.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class PickupHighlighter
    {
        private Dictionary<Pickup, Label> labels = new Dictionary<Pickup, Label>();
        private Dictionary<GameObject, Pickup> pickupsByGameObjects = new Dictionary<GameObject, Pickup>();
        private List<Pickup> toRemove = new List<Pickup>();
        private PointerEventData pointerData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();
        private Pickup hotPickup;

        public PickupHighlighter()
        {
            pointerData = new PointerEventData (EventSystem.current)
            {
                pointerId = -1,
            };
        }

        public void Show(ISet<Pickup> pickups)
        {
            SyncLabels(pickups);
            UpdatePositions();
            Hot = CalculateHotPickup();
        }

        public Pickup Hot
        {
            get => hotPickup;
            private set
            {
                if (hotPickup != null)
                {
                    if (labels.ContainsKey(hotPickup))
                        labels[hotPickup].Highlighed = false;
                    hotPickup.selected = false;
                }

                hotPickup = value;
                if (hotPickup != null)
                {
                    hotPickup.selected = true;
                    labels[hotPickup].Highlighed = true;
                }
            }
        }

        private void SyncLabels(ISet<Pickup> pickups)
        {
            toRemove.Clear();
            foreach (var pickup in labels.Keys)
            {
                if (!pickups.Contains(pickup))
                    toRemove.Add(pickup);
            }
            foreach (var pickup in toRemove)
            {
                Label label = labels[pickup];
                labels.Remove(pickup);
                pickupsByGameObjects.Remove(label.gameObject);
                label.Hide();  // TODO return to pool
            }
            foreach (var pickup in pickups)
            {
                if (!labels.ContainsKey(pickup))
                {
                    var label = new Label(null);
                    labels.Add(pickup, label);  // TODO get from pool
                    pickupsByGameObjects.Add(label.gameObject, pickup);
                }
            }
        }

        private void UpdatePositions()
        {
            foreach (KeyValuePair<Pickup, Label> entry in labels)
            {
                Pickup pickup = entry.Key;
                Label label = entry.Value;
                var position = pickup.transform.position + (Vector3) pickup.titleOffset / Iso.pixelsPerUnit;
                label.Show(position, pickup.title);
            }
        }

        private Pickup CalculateHotPickup()
        {
            pointerData.position = Input.mousePosition;

            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0)
                return null;

            var gameObject = raycastResults[0].gameObject;
            if (pickupsByGameObjects.TryGetValue(gameObject, out var pickup))
                return pickup;
            return null;
        }
    }
}
