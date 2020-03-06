using System.Collections.Generic;
using Diablerie.Engine.Entities;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Diablerie.Engine.UI
{
    public class LootHighlighter
    {
        private Dictionary<Loot, Label> labels = new Dictionary<Loot, Label>();
        private Dictionary<GameObject, Loot> lootsByGameObjects = new Dictionary<GameObject, Loot>();
        private List<Loot> toRemove = new List<Loot>();
        private PointerEventData pointerData;
        private List<RaycastResult> raycastResults = new List<RaycastResult>();
        private Loot _hotLoot;
        private LabelPool labelPool;
        private readonly LootLabelLayout layout = new LootLabelLayout(1);

        public LootHighlighter(LabelPool labelPool)
        {
            this.labelPool = labelPool;
            pointerData = new PointerEventData (EventSystem.current)
            {
                pointerId = -1,
            };
        }

        public void Show(ISet<Loot> loots, bool updateHot = true)
        {
            CreateLabels(loots);
            layout.Arrange(labels);
            if (updateHot)
                Hot = CalculateHotLoot();
        }

        public Loot Hot
        {
            get => _hotLoot;
            private set
            {
                if (_hotLoot != null)
                {
                    if (labels.ContainsKey(_hotLoot))
                        labels[_hotLoot].Highlighed = false;
                    _hotLoot.selected = false;
                }

                _hotLoot = value;
                if (_hotLoot != null)
                {
                    _hotLoot.selected = true;
                    labels[_hotLoot].Highlighed = true;
                }
            }
        }

        private void CreateLabels(ISet<Loot> loots)
        {
            toRemove.Clear();
            foreach (var loot in labels.Keys)
            {
                if (!loots.Contains(loot))
                    toRemove.Add(loot);
            }
            foreach (var loot in toRemove)
            {
                Label label = labels[loot];
                labels.Remove(loot);
                lootsByGameObjects.Remove(label.gameObject);
                labelPool.Return(label);
            }
            foreach (var loot in loots)
            {
                if (!labels.ContainsKey(loot))
                {
                    var label = labelPool.Get();
                    labels.Add(loot, label);
                    lootsByGameObjects.Add(label.gameObject, loot);
                }
            }
        }

        private Loot CalculateHotLoot()
        {
            pointerData.position = Input.mousePosition;

            EventSystem.current.RaycastAll(pointerData, raycastResults);

            if (raycastResults.Count == 0)
                return null;

            var gameObject = raycastResults[0].gameObject;
            return lootsByGameObjects.GetValueOrDefault(gameObject);
        }
    }
}
