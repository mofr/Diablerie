using System.Collections.Generic;
using Diablerie.Engine.Entities;
using UnityEngine;

namespace Diablerie.Engine.UI
{
    public class PickupHighlighter
    {
        private Dictionary<Pickup, Label> labels = new Dictionary<Pickup, Label>();
        private List<Pickup> toRemove = new List<Pickup>();

        public void Show(ISet<Pickup> pickups)
        {
            SyncLabels(pickups);
            UpdatePositions();
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
                label.Hide();  // TODO return to pool
                labels.Remove(pickup);
            }
            foreach (var pickup in pickups)
            {
                if (!labels.ContainsKey(pickup))
                {
                    labels.Add(pickup, new Label(null));  // TODO get from pool
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
    }
}
