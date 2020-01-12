using System.Collections.Generic;
using Diablerie.Engine.Entities;
using Diablerie.Engine.LibraryExtensions;
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
        private LabelPool labelPool;
        private readonly PickupLabelLayout layout = new PickupLabelLayout(1);

        public PickupHighlighter(LabelPool labelPool)
        {
            this.labelPool = labelPool;
            pointerData = new PointerEventData (EventSystem.current)
            {
                pointerId = -1,
            };
        }

        public void Show(ISet<Pickup> pickups, bool updateHot = true)
        {
            CreateLabels(pickups);
            layout.Arrange(labels);
            if (updateHot)
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

        private void CreateLabels(ISet<Pickup> pickups)
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
                labelPool.Return(label);
            }
            foreach (var pickup in pickups)
            {
                if (!labels.ContainsKey(pickup))
                {
                    var label = labelPool.Get();
                    labels.Add(pickup, label);
                    pickupsByGameObjects.Add(label.gameObject, pickup);
                }
            }
        }

        private Pickup CalculateHotPickup()
        {
            pointerData.position = Input.mousePosition;

            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0)
                return null;

            var gameObject = raycastResults[0].gameObject;
            return pickupsByGameObjects.GetValueOrDefault(gameObject);
        }
    }
}
